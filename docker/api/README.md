# droneflightlogapisqlite

The [DroneFlightLogDb](https://github.com/davewalker5/DroneFlightLogDb) and [DroneFlightLog.Api](https://github.com/davewalker5/DroneFlightLog.Api) GitHub projects implement the entities, business logic and a REST service for a SQL-based drone flight logbook, providing facilities for recording and querying the following data:

- Operator details
- Drones, models and manufacturers
- Flights, flight locations and flight properties

The droneflightlogapisqlite image contains a build of the logic and REST service for a SQLite database.

## Getting Started

### Prerequisities

In order to run this image you'll need docker installed.

- [Windows](https://docs.docker.com/windows/started)
- [OS X](https://docs.docker.com/mac/started/)
- [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Container Parameters

The following "docker run" parameters are recommended when running the droneflightlogapisqlite image:

| Parameter | Value                              | Purpose                                                 |
| --------- | ---------------------------------- | ------------------------------------------------------- |
| -d        | -                                  | Run as a background process                             |
| -v        | /local:/var/opt/droneflightlog.api | Mount the host folder containing the SQLite database    |
| -p        | 5001:80                            | Expose the container's port 80 as port 5001 on the host |
| --rm      | -                                  | Remove the container automatically when it stops        |

For example:

```shell
docker run -d -v  /local:/var/opt/droneflightlog.api/ -p 5001:80 --rm  davewalker5/droneflightlogapisqlite:latest
```

The "/local" path given to the -v argument is described, below, and should be replaced with a value appropriate for the host running the container. Similarly, the port number "5001" can be replaced with any available port on the host.

#### Volumes

The description of the container parameters, above, specifies that a folder containing the SQLite database file for the Drone Flight Log is mounted in the running container, using the "-v" parameter.

That folder should contain a SQLite database that has been created using the instructions in the [Drone Flight Log wiki](https://github.com/davewalker5/DroneFlightLogDb/wiki).

Specifically, the following should be done:

- [Create the SQLite database](https://github.com/davewalker5/DroneFlightLogDb/wiki/Using-a-SQLite-Database)
- [Add a user to the database](https://github.com/davewalker5/DroneFlightLogDb/wiki/REST-API)

The folder containing the "droneflightlog.db" file can then be passed to the "docker run" command using the "-v" parameter.

#### Running the Image

To run the image, enter the following command, substituting "/local" for the host folder containing the SQLite database, as described:

```shell
docker run -d -v  /local:/var/opt/droneflightlog.api/ -p 5001:80 --rm  davewalker5/droneflightlogapisqlite:latest
```

Once the container is running, browse to the following URL on the host:

http://localhost:5001

You should see the Swagger API documentation for the API.

## Find Us

- [DroneFlightLogDb on GitHub](https://github.com/davewalker5/DroneFlightLogDb)
- [DroneFlightLog.Api on GitHub](https://github.com/davewalker5/DroneFlightLog.Api)

## Versioning

For the versions available, see the [tags on this repository](https://github.com/davewalker5/DroneFlightLog.Api/tags).

## Authors

- **Dave Walker** - _Initial work_ - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

See also the list of [contributors](https://github.com/davewalker5/DroneFlightLog.Api/contributors) who
participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/davewalker5/DroneFlightLog.Api/blob/master/LICENSE) file for details.
