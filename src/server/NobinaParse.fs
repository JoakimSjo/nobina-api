module NobinaApi.Parse

open FParsec
open NobinaApi.Types
open NobinaApi.Shared

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

let attr: Parser<string * string, unit> = parse {
    let! key = many1 (noneOf "=") |>> implode
    do! pchar '=' |>> ignore
    let! value = wrap ((many (noneOf "\"")) |>> implode) (pstring "\"")
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
        let! attrs = manyTill parseNotes (lookAhead (pstring "</fromnotes>"))
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

let pZone = parse {
    let parseZone = parse {
        do! parseOpenTagNotClosed "zone"
        let! attrs = sepEndBy attr (pchar ' ')
        do! pstring "/>" |>> ignore
        return attrs
    }
    let parseZoneWs = between ws ws parseZone
    let! attrs = between (parseOpeningTag "zones") (parseClosingTag "zones") (manyTill parseZoneWs (lookAhead (pstring "</zones>")))
    return attrs
}

let pStages = parse {
    let! attrs = pOpenTagAttrs "i"
    do! pchar '>' |>> ignore
    do! ws
    let! zones = pZone
    do! parseClosingTag "i"
    return (attrs, zones)
}

let parseStages = parse {
    let! stages = between (parseOpeningTag "stages") (parseClosingTag "stages" ) (manyTill pStages (lookAhead (pstring "</stages>")))
    return stages
}

let toResult =
    function
    | Success (p, _, _) -> Result.Ok p
    | Failure _ as err -> 
            printfn "%A" err            
            Result.Error err

let findValue key map =
    match Map.tryFind key map with
    | Some s -> s
    | None -> "Ikke funnet"

let getTime map = 
    let time = findValue "d2" map
    if time = "Ikke funnet" then findValue "d" map else time     

let getStopNumber map = 
    let stopNumber = findValue "stopnr" map
    match stopNumber with
    | "Ikke funnet" -> 0
    | _ -> stopNumber |> int

let toDepartureNote notes : DepartureNote =
    let description = findValue "d" notes
    let situation = findValue "st" notes
    let version = findValue "sv" notes
    { description = description; situation=situation; version=version}

let toDeparture (res: (Info * Notes )) : Departure =
    let (Info info), (Notes notes) = res
    let line = findValue "l" info
    let time = getTime info
    let description = findValue "nd" info
    let live = Map.containsKey "d2" info
    let stopNumber = getStopNumber info
    let parsedNote = match notes with
                     | Some n -> (List.map toDepartureNote n)
                     | None -> []

    { route = description; line = line; live = live; time = time; notes = parsedNote; stopNumber = stopNumber}

let toZone zone : Zone =
    let zone' = Map.ofList zone
    let value = findValue "v" zone'
    let region = findValue "n" zone'

    {value = value; region = region}

let toStop (res: (string * string) list * (string * string) list list) =
    let a, b = res
    let stage = Map.ofList a
    let stopId = findValue "hplnr" stage |> int
    let stopNumber = getStopNumber stage
    let longitude = findValue "x" stage 
    let latitude = findValue "y" stage
    let lines = findValue "l" stage |> fun (s:string) -> s.Split[|','|]
    let zones = List.map toZone b |> Array.ofList

    {stopId = stopId; stopNumber = stopNumber; longitude = longitude; latitude = latitude; lines = lines; zones = zones;}

let toStopSearch (res: (string * string) list) (stops: Stop list) =
    let a = res
    let stop = Map.ofList a
    let name = findValue "n" stop
    let distance = findValue "d" stop |> int
    let longitude = findValue "x" stop 
    let latitude = findValue "y" stop
    {name = name; distance = distance; longitude = longitude; latitiude = latitude; stops = Array.ofList stops}

let toMap (res:(string * string) list * (string * string) list list option) =
    let a, b = res
    let a' = Map.ofList a |> Info
    let b' = b
             |> Option.map (fun a -> List.map Map.ofList a) |> Notes
    (a', b')

let parseDepartures = parse {
    do! parseOpeningTag "?xml version=\"1.0\" encoding=\"UTF-8\" ?"
    do! parseOpeningTag "result"
    let! deps = between (parseOpeningTag "departures") (parseClosingTag "departures") (manyTill pDeparture (lookAhead (pstring "</departures>")))
    let! stages = between (parseOpeningTag "stages") (parseClosingTag "stages" ) (manyTill pStages (lookAhead (pstring "</stages>")))
    return {departures = deps |> List.map (toMap >> toDeparture); stops = stages |> List.map toStop} 
}

let parseNearestStation = parse {
    do! parseOpeningTag "?xml version=\"1.0\" encoding=\"UTF-8\" ?"
    let pGroup = parse {
        let! attrs = between (parseOpenTagNotClosed "group") (pstring ">") (sepEndBy attr (pchar ' '))
        let! stages = (manyTill pStages (lookAhead (pstring "</group>")))
        do! parseClosingTag "group"
        return toStopSearch attrs (stages |> List.map toStop)
    }

    let! groups = between (parseOpeningTag "stages") (parseClosingTag "stages") (manyTill pGroup (lookAhead (pstring "</stages>")))
    return groups
}

let parseNobinaResponse = 
    (toResult << run parseDepartures)

let parseNobinaNearestStationResponse =
    (toResult << run parseNearestStation)