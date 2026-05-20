# Tatar Food Delivery API

Репозиторий для проекта по ООП: сервис заказа татарской еды с микросервисной архитектурой.

Проект состоит из трех ASP.NET Core сервисов:

- `TatarDelivery.UserService`
- `TatarDelivery.CatalogService`
- `TatarDelivery.OrderService`

## Что реализовано

`User Service` поддерживает:

- `POST /users/register`
- `POST /users/login`
- `GET /users/me`
- `PUT /users/me`
- `GET /users/me/addresses`
- `POST /users/me/addresses`

`Catalog Service` поддерживает:

- `GET /categories`
- `GET /dishes`
- `GET /dishes/{id}`
- `POST /dishes`

`Order Service` поддерживает:

- `POST /orders`
- `GET /orders/{order_id}`
- `GET /orders/my`
- `POST /orders/{order_id}/cancel`
- `POST /orders/{order_id}/pay`
- `POST /orders/{order_id}/deliver`

Внутри есть:

- регистрация и логин;
- просмотр и редактирование профиля;
- добавление и просмотр адресов доставки;
- валидация входных данных;
- обработка ошибок;
- хранение данных в `SQLite`;
- Swagger UI для проверки ручек.

## Стек

- `ASP.NET Core`
- `Entity Framework Core`
- `SQLite`
- `Swagger / Swashbuckle`

## Запуск

### 1. Поднять PostgreSQL для Catalog Service

```bash
docker compose up -d catalog-db
```

### 2. Собрать solution

```bash
dotnet build oop-tatar-delivery-api.sln
```

### 3. Запустить сервисы

В трех отдельных терминалах:

```bash
dotnet run --project TatarDelivery.UserService/TatarDelivery.UserService.csproj
```

```bash
dotnet run --project TatarDelivery.CatalogService/TatarDelivery.CatalogService.csproj
```

```bash
dotnet run --project TatarDelivery.OrderService/TatarDelivery.OrderService.csproj
```

Swagger будет доступен:

- User Service: `http://localhost:5100/swagger`
- Catalog Service: `http://localhost:5078/swagger`
- Order Service: `http://localhost:5007/swagger`

## Авторизация в Swagger

1. Вызвать `POST /users/register`.
2. Вызвать `POST /users/login`.
3. Скопировать `accessToken` из ответа.
4. Нажать `Authorize` и вставить токен.

## База данных

Локальный файл базы создаётся автоматически:

- `TatarDelivery.UserService/tatar-delivery-users.db`

Таблицы:

- `Users`
- `Addresses`

`Catalog Service` использует PostgreSQL из `docker-compose.yml`.

`Order Service` использует локальный SQLite-файл и mock-цены блюд.
