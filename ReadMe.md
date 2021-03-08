# PostgreSqlDistributedLock 
This project demonstrate how you can create a Distributed Lock in PostgreSQL. Please refer to my [blog post](https://ankitvijay.net/2021/02/28/distributed-lock-using-postgresql/) post to follow the code.



## Pre-requisites to run the tests
- Docker

## Steps to run the tests
- Create a sample PostgresSQL database using below command:
> docker run --name distributed-lock-test -e POSTGRES_USER=dbUser -e POSTGRES_DB=distributed-lock-db -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres
