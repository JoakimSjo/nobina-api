module NobinaApi.Api

open Giraffe.ResponseWriters
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

open NobinaApi.Departures

let departures : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        let logger = ctx.GetLogger("Departures")
        let stop = ctx.TryGetQueryStringValue "stop"
                   |> Option.defaultValue "None"
        if stop <> "None" then
            logger.LogInformation(sprintf "Requested departures for stop %s" stop)
            let result = getDepartures stop
            json result next ctx
        else
            ctx.SetStatusCode 400
            next ctx      