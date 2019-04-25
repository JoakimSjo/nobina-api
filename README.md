# Nobina API
Simple API that returns the bus schedules for bus stops in Tromsø.

Currently the only endpoint is **/nobina/departures?stop=Raboof** and it will return all **departures** and **stops** from the station **Raboof** in JSON format.

Example response:

```json
{
  "departures": [
    {
      "route": "UiT - UNN via Giæverbukta",
      "line": "33",
      "time": "25.04.2019 22:47:27",
      "live": true,
      "stopNumber": 2,
      "notes": []
    },
    {
      "route": "UiT - UNN via Giæverbukta",
      "line": "33",
      "time": "25.04.2019 22:59:12",
      "live": true,
      "stopNumber": 2,
      "notes": []
    },
    {
      "route": "UiT - UNN via sentrum",
      "line": "34",
      "time": "25.04.2019 23:03:37",
      "live": true,
      "stopNumber": 1,
      "notes": []
    },
    {
      "route": "UiT - UNN via sentrum",
      "line": "34",
      "time": "25.04.2019 23:31:00",
      "live": true,
      "stopNumber": 1,
      "notes": []
    }
  ],
  "stops": [
    {
      "stopId": 19021102,
      "stopNumber": 1,
      "longitude": "18,934237",
      "latitude": "69,637518",
      "lines": [
        "34"
      ],
      "zones": [
        {
          "value": "19100",
          "region": "Tromsø"
        },
        {
          "value": "19100",
          "region": "Tromsø"
        }
      ]
    },
    {
      "stopId": 19021102,
      "stopNumber": 2,
      "longitude": "18,934236",
      "latitude": "69,637653",
      "lines": [
        "33"
      ],
      "zones": [
        {
          "value": "19100",
          "region": "Tromsø"
        },
        {
          "value": "19100",
          "region": "Tromsø"
        }
      ]
    }
  ]
}
```

# Usage

To build run **dotnet restore** and **dotnet build** inside the ***src/server*** folder. Start the server with **dotnet run**. Or use **docker-compose build** inside ***src/server***. 

# Misc

The main purpose of this project is to learn FSharp, therefore e.g the response from the Nobina API is parsed using a custom parser written in FParsec. 

Please contribute and report issues 😍🙌🏻

