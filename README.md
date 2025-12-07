# DroneFlightLog

[![Build Status](https://github.com/davewalker5/DroneFlightLog/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/DroneFlightLog/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/DroneFlightLog)](https://github.com/davewalker5/DroneFlightLog/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/DroneFlightLog/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/DroneFlightLog?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/DroneFlightLog.svg?include_prereleases)](https://github.com/davewalker5/DroneFlightLog/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/DroneFlightLog/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/DroneFlightLog/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/DroneFlightLog)](https://github.com/davewalker5/DroneFlightLog/)

## About

DroneFlightLog implements a SQL-based drone flight log boook. An ASP.NET WebAPI provides access to the business logic and data access layer while an ASP.NET MVC UI provides the user interface.

The logbook allows for storage and maintenance of the following data:

- Operator details
- Drones, models and manufacturers
- Flights, flight locations and flight properties
- Repair, maintenance and modification records

## Getting Started

Please see the [Wiki](https://github.com/davewalker5/DroneFlightLog/wiki) for configuration details and the user guide.

## Authors

- **Dave Walker** - _Initial work_

## Credits

Implementation of authentication using JWT in the REST API is based on the following tutorial:

- https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api
- https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api#users-controller-cs

The Drone Flight Log MVC project uses the Gijgo JavaScript controls library:

- https://gijgo.com
  
## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/DroneFlightLog/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
