module  NobinaApi.Departures
open FSharp.Data
open NobinaApi.Parse
open NobinaApi.Types

[<Literal>]
let NobinaDeparturesUrl = "http://rp.tromskortet.no/scripts/TravelMagic/TravelMagicWE.dll/v1DepartureSearchXML?hpl="

let generateUrl stop = 
    NobinaDeparturesUrl + stop + "%20%28Troms%C3%B8%29&now=1&realtime=1"

let getDepartures stop = 
    let requestPath = generateUrl stop
    let response = Http.Request(requestPath, silentHttpErrors = true)

    let text = match response.Body with 
               | Text s -> s
               | _ -> "Error with response"

    match parseNobinaResponse text with
    | Ok deps -> deps
    | _ -> []