# Test report

Date: 20.05.2026

## Environment

- OS/user workspace: local development environment.
- .NET SDK: 8.0.126.
- Service checked: `TatarDelivery.UserService`.
- Base URL: `http://localhost:5100`.
- Swagger URL: `http://localhost:5100/swagger`.

## Build result

Initial build failed because `TatarDelivery.UserService.csproj` targeted `net10.0`,
while the local machine had only .NET SDK 8 installed.

Fix applied:

- changed target framework from `net10.0` to `net8.0`;
- changed `Microsoft.EntityFrameworkCore.Sqlite` from `10.0.8` to `8.0.22`.

After that, `dotnet build` finished successfully.

## Passed checks

- `POST /users/register` returns `201 Created` for valid data.
- `POST /users/login` returns `200 OK` and `accessToken`.
- `GET /users/me` returns current user when `Authorization: Bearer <token>` is provided.
- `PUT /users/me` updates current user profile.
- `POST /users/me/addresses` returns `201 Created` for valid address data.
- `GET /users/me/addresses` returns created addresses.
- Duplicate `POST /users/register` with the same email returns `409 Conflict`.
- `POST /users/login` with a wrong password returns `401 Unauthorized`.
- `POST /users/register` with invalid data returns `400 Bad Request`.
- `GET /users/me` without authorization returns `401 Unauthorized`.
- `POST /users/me/addresses` with invalid data returns `400 Bad Request`.

## Not checked yet

- `Catalog Service`: project is not present in the repository yet.
- `Order Service`: project is not present in the repository yet.

## Blockers

### BLOCKER-001: Catalog and Order services are absent

The README lists `Catalog Service` and `Order Service` endpoints, but the repository
currently contains only `TatarDelivery.UserService`.

Impact:

- full demo scenario cannot be completed;
- Postman collection for catalog and orders can only be prepared as a draft;
- integration checks cannot be run yet.

Expected:

- add `Catalog Service` project;
- add `Order Service` project;
- document their local ports and Swagger URLs.
