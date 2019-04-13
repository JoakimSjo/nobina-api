module NobinaApi.Parse

open FParsec
open NobinaApi.Types

type Info = Info of Map<string, string>
type Notes = Notes of Map<string, string> list option

let ws =
    spaces

let implode =
    List.fold (fun curr x -> curr + string (x)) ""

let wrap p wrapper = parse {
    do! wrapper |>> ignore
    let! res = p
    do! wrapper |>> ignore
    return res
}

let allowedChars =
    ['A' .. 'Z'] @ ['a' .. 'z'] @ ['0' .. '9'] @ ['æ'; 'ø'; 'å'; 'Æ'; 'Ø'; 'Å'; '.'; ','; ' '; ':'; '-'; '!']

let attr: Parser<string * string, unit> = parse {
    let! key = many1 (anyOf allowedChars) |>> implode
    do! pchar '=' |>> ignore
    let! value = wrap ((many (anyOf allowedChars)) |>> implode) (pstring "\"")
    return (key, value)
}

let parseTagAndws opener closer tag =
    parse {
        do! ws
        do! opener |>> ignore
        do! (pstring tag) |>> ignore
        do! closer |>> ignore
        do! ws
    }

let parseOpeningTag =
    parseTagAndws (pstring "<") (pstring ">")

let parseClosingTag =
    parseTagAndws (pstring "</") (pstring ">")

let parseOpenTagNotClosed =
    parseTagAndws (pstring "<") (preturn None)

let pOpenTagAttrs tag = parse {
    do! (parseOpenTagNotClosed tag) |>> ignore
    let! attrs = sepBy attr (pchar ' ')
    do! ws
    return attrs
}

let pOptionalAndCheck optTag = parse {
    let! tag = opt (pstring optTag)
    match tag with
    | Some s -> return s = optTag
    | None -> return false
}

let pNotes = parse {
    let! fromTagExists = pOptionalAndCheck "<fromnotes>"

    let parseNotes = parse {
        do! parseOpenTagNotClosed "i"
        let! attrs = sepEndBy attr (pchar ' ')
        do! pstring "/>" |>> ignore
        return attrs
    }

    if fromTagExists then
        let! attrs = manyTill parseNotes (lookAhead  (pstring "</fromnotes>"))
        do! parseClosingTag "fromnotes"
        return Some attrs
    else
        return! preturn None
}

let pDeparture = parse {
    let! attrs = pOpenTagAttrs "i"
    let! isSelfClosing = pOptionalAndCheck "/"
    do! pchar '>' |>> ignore
    do! ws
    if isSelfClosing then
        return (attrs, None)
    else
         let! notes = pNotes
         do! parseClosingTag "i"
         return (attrs, notes)
}

let parseRes = parse {
    do! parseOpeningTag "?xml version=\"1.0\" encoding=\"UTF-8\" ?"
    do! parseOpeningTag "result"
    do! parseOpeningTag "departures"
    let! deps = manyTill pDeparture (lookAhead (pstring "</departures>"))
    return deps
}

let toResult =
    function
    | Success (p, _, _) -> Result.Ok p
    | Failure _ as err -> 
            printfn "%A" err            
            Result.Error err

let findValue key map =
    let result = Map.tryFind key map
    match result with
    | Some s -> s
    | None -> "Ikke funnet"

let getTime map = 
    let time = findValue "d2" map
    if time = "Ikke funnet" then findValue "d" map else time     

let toDepartureNote notes : DepartureNote =
    let description = findValue "d" notes
    let situation = findValue "st" notes
    let version = findValue "sv" notes
    { description = description; situation=situation; version=version}

let toDeparture (res: (Info * Notes)) : Departure =
    let (Info info), (Notes notes) = res
    let line = findValue "l" info
    let time = getTime info
    let description = findValue "nd" info
    let parsedNote = match notes with
                     | Some n -> Some (List.map toDepartureNote n)
                     | None -> None
    { departure = description; line = line; time = time; note = parsedNote}

let toMap (res:(string * string) list * (string * string) list list option) =
    let a, b = res
    let a' = Map.ofList a |> Info
    let b' = b
             |> Option.map (fun a -> List.map Map.ofList a) |> Notes
    (a', b')

let parseNobinaResponse text = 
    run parseRes text |> toResult |> Result.map (List.map (toMap >> toDeparture))
