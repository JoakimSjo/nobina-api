module NobinaApi.Shared

open NobinaApi.Types

type DepartureResult = {
    departures: Departure list
    stops: Stop list
}