module NobinaApi.Api

open Giraffe.ResponseWriters
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

open NobinaApi.Departures
open NobinaApi.Stops

let departures : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        let logger = ctx.GetLogger("Departures")
        let stop = ctx.TryGetQueryStringValue "stop"
                   |> Option.defaultValue "None"
        if stop <> "None" then
            logger.LogInformation(sprintf "Requested departures for stop %s" stop)
            json (getDepartures stop) next ctx
        else
            ctx.SetStatusCode 400
            next ctx      

let stops : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger("Nearest stops")
        let longitude = ctx.TryGetQueryStringValue "longitude"
        let latitude = ctx.TryGetQueryStringValue "latitude"

        match (longitude, latitude) with 
        | Some lo, Some la ->  
                   logger.LogInformation(sprintf "Reqesting stops near %s, %s" lo la)
                   json (getNearestStop lo la logger) next ctx
        | Some _, None ->
                   ctx.SetStatusCode 400
                   text "ERROR missing latitiude argument" next ctx
        | None, Some _ ->
                   ctx.SetStatusCode 400
                   text "ERROR missing longitude argument" next ctx
        | _, _ ->  ctx.SetStatusCode 400
                   text "Error missing longitude and latitude argument" next ctx                                                                                             