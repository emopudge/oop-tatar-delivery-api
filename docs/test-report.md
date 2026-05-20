# Test report

Date: 20.05.2026

## Environment

- .NET SDK: 8.0.126.
- PostgreSQL: `docker compose up -d catalog-db`.
- User Service: `http://localhost:5100`.
- Catalog Service: `http://localhost:5078`.
- Order Service: `http://localhost:5007`.

## Build

`dotnet build oop-tatar-delivery-api.sln` passes.

## Swagger

- User Service Swagger: passed.
- Catalog Service Swagger: passed.
- Order Service Swagger: passed.

## User Service

Passed:

- registration;
- duplicate email conflict;
- login;
- wrong password;
- profile read;
- profile update;
- address creation;
- address list;
- validation errors;
- unauthorized profile request.

## Catalog Service

Passed:

- `GET /categories`;
- `GET /dishes`;
- `GET /dishes?categoryId=2`;
- `GET /dishes?search=%D1%87%D0%B0%D0%BA`;
- `GET /dishes/9999` returns `404`;
- seed data contains 12 dishes.

## Order Service

Passed:

- `POST /orders`;
- `GET /orders/{id}`;
- `GET /orders/my?userId=1`;
- `POST /orders/{id}/pay`;
- `POST /orders/{id}/deliver`;
- `POST /orders/{id}/cancel` for `PendingPayment`;
- cancel after payment returns `400`;
- empty `items` returns `400`;
- unknown order returns `404`.

## Notes

- `Catalog Service` needs PostgreSQL before startup.
- `Order Service` uses mock dish prices, so order totals are calculated independently from Catalog Service prices.
- Error response format differs between Catalog Service and User/Order services.
