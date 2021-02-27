# Pre-requisites
- Docker

# Steps to run the tests
- Create a sample PostgresSQL database using below command:
> docker run --name distributed-lock-test -e POSTGRES_USER=dbUser -e POSTGRES_DB=distributed-lock-db -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres