module NobinaApi.Misc

open System

open Microsoft.AspNetCore
open Microsoft.Extensions.Logging
open Giraffe
open NobinaApi.Api

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    htmlString greetings

let webApp : HttpFunc -> Http.HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                routef "/nobina/%s" departures
            ]
        setStatusCode 404 >=> text "Not Found" ]