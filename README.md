# Nobina API
Simple API that can return bus schedules from a bus stop and find bus stops near a position.

## Departures from a bus stop 
| Request     |       |
|  ---  |  ---  |
| Method | **Get**
|   Path    |   **/nobina/departures**    |
|   URL Params    |   **Required**<br /> stop=[string] <br/>Example: *stop = Raboof*   |

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
      "longitude": 18.934237,
      "latitude": 69.637518,
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
      "longitude": 18.934236,
      "latitude": 69.637653,
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

## Bus stops near a position
| Request     |       |
|  ---  |  ---  |
| Method | **Get**
|   Path    |   **/nobina/nearestStops**    |
|   URL Params    |   **Required**<br /> longitude=[Double] <br/>**Required**<br /> latitude=[Double]   |




Example response:
```json
[
  {
    "name": "Peder Hansens gate (Tromsø)",
    "distance": 56,
    "longitude": 18.951653,
    "latitiude": 69.646272,
    "stops": [
      {
        "stopId": 19021092,
        "stopNumber": 1,
        "longitude": 18.951653,
        "latitude": 69.646272,
        "lines": [
          "37"
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
  },
  {
    "name": "Fiskergata (Tromsø)",
    "distance": 107,
    "longitude": 18.953027,
    "latitiude": 69.647139,
    "stops": [
      {
        "stopId": 19021090,
        "stopNumber": 1,
        "longitude": 18.953027,
        "latitude": 69.647139,
        "lines": [
          "33",
          "42"
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
]
```

# Usage

To build run **dotnet restore** and **dotnet build** inside the ***src/server*** folder. Start the server with **dotnet run**. Or use **docker-compose build** inside ***src/server***. Default port is **8085**, but this can be changed by defining the enviroment variable *SERVER_PORT* with your preferred port.

# Misc

The main purpose of this project is to learn FSharp, therefore e.g the response from the Nobina API is parsed using a custom parser written in FParsec. 

Please contribute and report issues 😍🙌🏻

