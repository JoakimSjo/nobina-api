# Nobina API
Simple API that returns the bus schedules for bus stops in Tromsø. 

Currently the only endpoint is **/nobina/departures?stop=Raboof** and it will return all departures from the station **Raboof** in a JSON format. 

Example response: 

```
[
    {
        "route": "Giæverbukta via Fagereng",
        "line": "33",
        "time": "14.04.2019 01:32:22",
        "live": true,
        "notes": []
    },
    {
        "route": "Eidkjosen via Giæverbukta",
        "line": "42",
        "time": "14.04.2019 01:32:22",
        "live": true,
        "notes": []
    },
    {
        "route": "Stakkevollan via sentrum",
        "line": "42",
        "time": "14.04.2019 02:25:00",
        "live": true,
        "notes": [
            {
                "description": "Bruk alternative holdeplasser ved Nerstranda linje 42 og Teorifagbygget linje 34",
                "situation": "situation",
                "version": "normal"
            }
        ]
    },
]
```

# Usage

To build run **dotnet restore** and **dotnet build** inside the ***src/server*** folder. Or use **docker-compose build** inside ***src/server***. 

# Misc

The main purpose of this project is to learn FSharp, therefore e.g the response from the Nobina API is parsed using a custom parser written in FParsec. 

Please contribute and report issues 😍🙌🏻

