module NobinaApi.Types

type DepartureNote = {
    description: string
    situation: string
    version: string
}

type Departure = {
    route : string
    line: string
    time: string
    notes: DepartureNote list
}