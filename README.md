# Journey Task

This repository contains a demo .NET solution composed of two services:

- **Journey.API** – ASP.NET Core Web API used for managing user journeys.
- **Reward.Worker** – background worker that consumes messages from RabbitMQ and publishes reward events.

The solution uses Docker Compose to run the services along with PostgreSQL and RabbitMQ.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose installed

## Running the services

Build and start the application using Docker Compose:

```bash
docker-compose up --build
```

This command starts PostgreSQL, RabbitMQ, the Journey API and the Reward Worker. The API will be available on <http://localhost:5001>.

## Solution structure

```
services/
  JourneyService/
    Journey.API/           # ASP.NET Core API project
    Journey.Application/   # Application layer
    Journey.Domain/        # Domain entities
    Journey.Infrastructure/# Infrastructure and data access

  RewardService/
    Reward.Worker/         # Background worker service
    Reward.Application/    # Application layer
    Reward.Domain/         # Domain entities
    Reward.Infrastructure/ # Infrastructure and data access
```

## Database migrations

Entity Framework Core migrations run automatically when the services start. Default database credentials are defined in `docker-compose.yml`.

## Health checks

Health endpoints are exposed at `/healthz` and `/readyz` in the Journey API.

---

This README provides a minimal overview for getting the services running locally. For full details, inspect the source code inside the `services/` folder
