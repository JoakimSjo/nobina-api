module NobinaApi.Types

type DepartureNote = {
    description: string
    situation: string
    version: string
}

type Departure = {
    departure : string
    line: string
    time: string
    note: DepartureNote list option
}