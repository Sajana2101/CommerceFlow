# CommerceFlow

CommerceFlow is a distributed e-commerce and order processing backend built with C# and .NET. The project was designed to demonstrate practical backend engineering concepts such as authentication, product management, shopping carts, order processing, inventory reservation, payments, event-driven messaging, caching, containerisation, and service separation.

The system uses a layered architecture and integrates SQL Server, Redis, RabbitMQ, Docker, and JWT authentication to simulate the core backend of a modern e-commerce platform.

## Features

* Product catalogue management
* Customer registration and login
* JWT-based authentication and authorization
* Role-based access control
* Redis-backed shopping cart
* Order creation and checkout
* Inventory management
* Atomic stock reservation during checkout
* Payment processing simulation
* Idempotent payment requests
* Payment success and failure handling
* RabbitMQ event-driven messaging
* Separate notification worker service
* SQL Server persistence with Entity Framework Core
* Docker and Docker Compose support
* RESTful API architecture

## Technology Stack

### Backend

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Authentication
* JWT Bearer Authentication

### Distributed Infrastructure

* Redis
* RabbitMQ
* Docker
* Docker Compose

### Frontend

* React
* TypeScript
* Vite

The React frontend is currently in an early development stage. The main focus of this project is the distributed backend architecture.

## Project Structure

```text
CommerceFlow
│
├── CommerceFlow.Api
│   └── REST API, controllers, authentication and application startup
│
├── CommerceFlow.Application
│   └── Application services, interfaces, DTOs and business use cases
│
├── CommerceFlow.Domain
│   └── Core domain entities, enums and business rules
│
├── CommerceFlow.Infrastructure
│   └── Entity Framework Core, SQL Server, Redis, RabbitMQ and repositories
│
├── CommerceFlow.NotificationService
│   └── Background worker that consumes RabbitMQ payment events
│
├── commerceflow-client
│   └── React and TypeScript frontend
│
├── docker-compose.yml
└── CommerceFlow.sln
```

## Architecture

CommerceFlow follows a layered architecture:

```text
Client
   │
   ▼
CommerceFlow.Api
   │
   ▼
CommerceFlow.Application
   │
   ▼
CommerceFlow.Domain
   │
   ▼
CommerceFlow.Infrastructure
```

Infrastructure services include:

```text
                         CommerceFlow.Api
                                │
             ┌──────────────────┼──────────────────┐
             │                  │                  │
             ▼                  ▼                  ▼
        SQL Server            Redis             RabbitMQ
                                                   │
                                                   ▼
                                  CommerceFlow.NotificationService
```

The API handles HTTP requests and delegates business operations to the application layer.

The application layer defines services and interfaces used by the API.

The domain layer contains the core business entities and rules.

The infrastructure layer provides implementations for database access, caching, messaging and external infrastructure.

## Core Domain

### Products

Products contain information such as:

* Product ID
* Name
* Description
* SKU
* Price
* Active status
* Creation date

Products support creation, updates and soft deletion.

### Customers

Customers can register and authenticate using email and password.

Passwords are securely hashed before being stored.

Customer roles include:

```text
Customer
Admin
```

JWT tokens are issued after successful registration or login.

### Shopping Cart

Shopping carts are stored in Redis rather than SQL Server.

Each cart belongs to a customer and contains:

* Product
* Quantity
* Unit price
* Line total
* Cart subtotal

Redis provides fast temporary storage for cart data.

### Orders

Customers can convert their shopping cart into an order.

Orders contain snapshots of product information so that historical orders remain accurate even if product information changes later.

Order statuses include:

```text
PendingPayment
Processing
Paid
Shipped
Delivered
Cancelled
PaymentFailed
```

### Inventory

Each product has an inventory record containing:

```text
AvailableQuantity
ReservedQuantity
TotalQuantity
```

When checkout occurs, CommerceFlow atomically reserves inventory.

For example:

```text
Before checkout

Available: 20
Reserved: 0

Customer orders 2 products

Available: 18
Reserved: 2
```

This prevents multiple customers from purchasing the same remaining inventory concurrently.

### Payments

CommerceFlow includes a simulated payment gateway.

Two development payment tokens are supported:

```text
tok_success
tok_fail
```

`tok_success` simulates a successful payment.

`tok_fail` simulates a failed payment.

Successful payments:

* Mark the payment as successful
* Mark the order as paid
* Complete the inventory reservation

Failed payments:

* Mark the payment as failed
* Mark the order as payment failed
* Release reserved stock back into available inventory

## Idempotent Payments

Payment requests require an idempotency key.

Example:

```json
{
  "idempotencyKey": "payment-001",
  "paymentMethodToken": "tok_success"
}
```

If the same request is accidentally submitted multiple times using the same idempotency key, CommerceFlow returns the existing payment instead of processing another payment.

This helps prevent duplicate charges.

## Event-Driven Messaging

CommerceFlow uses RabbitMQ for asynchronous communication.

After a payment is processed, the API publishes events such as:

```text
payment.succeeded
payment.failed
```

These events are published to the:

```text
commerceflow.events
```

RabbitMQ topic exchange.

Consumers can independently respond to these events without being tightly coupled to the payment process.

## Notification Service

`CommerceFlow.NotificationService` is a separate .NET Worker Service.

It listens for RabbitMQ payment events.

For example:

```text
CommerceFlow.Api
       │
       │ payment.succeeded
       ▼
    RabbitMQ
       │
       ▼
NotificationService
```

When a successful payment event is received, the service currently logs a simulated customer notification.

This service demonstrates how additional microservices can subscribe to CommerceFlow events without modifying the core payment workflow.

## API Endpoints

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/account
```

### Products

```http
GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}
```

Product catalogue queries support:

* Search
* Minimum price
* Maximum price
* Sorting
* Pagination

Example:

```http
GET /api/products?search=keyboard&minPrice=500&maxPrice=2000&sortBy=price&sortDirection=desc&page=1&pageSize=20
```

### Cart

```http
GET    /api/cart
POST   /api/cart/items
PUT    /api/cart/items/{productId}
DELETE /api/cart/items/{productId}
DELETE /api/cart
```

### Orders

```http
POST /api/orders/checkout
GET  /api/orders
GET  /api/orders/{id}
```

### Payments

```http
POST /api/orders/{orderId}/payment
```

### Inventory

```http
GET /api/inventory/{productId}
PUT /api/inventory/{productId}
```

Updating inventory requires an Admin role.

## Running CommerceFlow with Docker

### Requirements

Install:

* Docker Desktop
* Git

Clone the repository:

```bash
git clone https://github.com/Sajana2101/CommerceFlow.git
```

Enter the project directory:

```bash
cd CommerceFlow
```

Build and start the complete system:

```bash
docker compose up --build
```

Docker Compose starts:

```text
CommerceFlow.Api
CommerceFlow.NotificationService
SQL Server
Redis
RabbitMQ
```

The API is available at:

```text
http://localhost:8080
```

RabbitMQ Management is available at:

```text
http://localhost:15672
```

Stop the application with:

```bash
docker compose down
```

## Docker Services

The Docker environment contains the following services:

| Service             | Purpose                         |
| ------------------- | ------------------------------- |
| CommerceFlow.Api    | Main REST API                   |
| SQL Server          | Persistent application database |
| Redis               | Shopping cart cache             |
| RabbitMQ            | Event broker                    |
| NotificationService | Payment event consumer          |

Persistent Docker volumes are used so database data remains available after containers are stopped.

## Example Checkout Flow

A typical CommerceFlow transaction works as follows:

```text
1. Customer registers or logs in
                │
                ▼
2. Customer browses products
                │
                ▼
3. Product added to Redis cart
                │
                ▼
4. Customer checks out
                │
                ▼
5. Inventory reserved atomically
                │
                ▼
6. Order created
                │
                ▼
7. Payment request submitted
                │
          ┌─────┴─────┐
          │           │
       Success      Failure
          │           │
          ▼           ▼
      Order Paid   Payment Failed
          │           │
          ▼           ▼
 Stock consumed   Stock released
          │
          ▼
RabbitMQ payment.succeeded
          │
          ▼
NotificationService
```

## Security

CommerceFlow currently implements several security practices:

* Password hashing
* JWT authentication
* Role-based authorization
* Protected customer endpoints
* Admin-only inventory modification
* Entity Framework parameterised database access
* Server-side validation
* Payment idempotency
* Generic login failure responses
* Payment method tokens are not stored

Sensitive JWT signing keys can be stored using .NET User Secrets during local development.

## Database

CommerceFlow uses SQL Server through Entity Framework Core.

Main tables include:

```text
Products
Customers
Orders
OrderItems
InventoryItems
Payments
```

Entity Framework migrations are used to create and update the database schema.

## Redis

Redis stores customer shopping carts using keys similar to:

```text
cart:{customerId}
```

Cart entries use a sliding expiration so inactive cart data does not remain indefinitely.

## RabbitMQ

RabbitMQ provides asynchronous communication between CommerceFlow services.

Current routing keys include:

```text
payment.succeeded
payment.failed
```

This architecture allows additional consumers to be introduced later, such as:

* Email notifications
* Order analytics
* Fraud detection
* Warehouse processing
* Customer activity tracking

## Future Improvements

Possible future improvements include:

* Complete React storefront
* Admin dashboard
* Product images
* Real payment provider integration
* Email notification provider
* Order shipping workflow
* Outbox pattern
* Dead-letter queues
* Automated testing
* CI/CD pipeline
* Cloud deployment
* Centralised logging
* Metrics and distributed tracing
* API gateway
* Additional microservices

## Skills Demonstrated

This project demonstrates practical experience with:

* ASP.NET Core Web API development
* C# object-oriented programming
* Domain modelling
* Layered architecture
* REST API design
* Entity Framework Core
* SQL Server
* Authentication and authorization
* JWT
* Redis caching
* Distributed systems
* RabbitMQ
* Event-driven architecture
* Background workers
* Inventory concurrency
* Payment idempotency
* Docker
* Docker Compose
* Git and GitHub
* React and TypeScript fundamentals

## Author

Sajana Motheram

GitHub: [Sajana2101](https://github.com/Sajana2101)

## Project Status

The CommerceFlow distributed backend is functional and containerised.

Core backend workflows including authentication, products, carts, orders, inventory, payments, RabbitMQ messaging and notification processing have been implemented.

Frontend development is currently in progress.
