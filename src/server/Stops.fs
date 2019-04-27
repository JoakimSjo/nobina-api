module  NobinaApi.Stops
open FSharp.Data
open NobinaApi.Parse
open NobinaApi.Shared
open Microsoft.Extensions.Logging

[<Literal>]
let NobinaNearestStopUrl = "http://rp.tromskortet.no/scripts/TravelMagic/TravelMagicWE.dll/v1NearestStopsXML?"

let generateUrl longitude latitude = 
    sprintf "%sx=%s&y=%s" NobinaNearestStopUrl longitude latitude

let getNearestStop longitude latitude (logger: ILogger)= 
    let requestPath = generateUrl longitude latitude 
    let response = Http.Request(requestPath, silentHttpErrors = true)
    logger.LogInformation requestPath
    let text = match response.Body with 
               | Text s -> s
               | _ -> "Error with response"

    match parseNobinaNearestStationResponse text with
    | Ok res -> res
    | _ -> []