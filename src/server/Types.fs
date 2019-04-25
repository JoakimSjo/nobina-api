module NobinaApi.Types

type Zone = {
    value: string
    region: string
}

type Stop = {
    stopId: int
    stopNumber: int
    longitude: string 
    latitude: string
    lines: string []
    zones: Zone []
}

type DepartureNote = {
    description: string
    situation: string
    version: string
}

type Departure = {
    route : string
    line: string
    time: string
    live: bool
    notes: DepartureNote list
    busStopId: int
}