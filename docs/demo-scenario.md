# Demo scenario

## Цель

Показать основной пользовательский сценарий сервиса доставки татарской еды:
регистрация, авторизация, добавление адреса, просмотр каталога и создание заказа.

## Текущий статус

На 20.05.2026 в репозитории реализован и локально проверен `User Service`.
Проекты `Catalog Service` и `Order Service` в рабочей папке пока отсутствуют,
поэтому полный сценарий можно пройти только после их добавления в репозиторий.

## User Service

Base URL:

```text
http://localhost:5100
```

### 1. Register user

Endpoint:

```text
POST /users/register
```

Request:

```json
{
  "email": "demo.qa@example.com",
  "password": "Password123!",
  "fullName": "Demo User",
  "phone": "+79990000000"
}
```

Expected result:

```text
201 Created
```

### 2. Login

Endpoint:

```text
POST /users/login
```

Request:

```json
{
  "email": "demo.qa@example.com",
  "password": "Password123!"
}
```

Expected result:

```text
200 OK
```

Save `accessToken` from the response and use it in protected requests:

```text
Authorization: Bearer <accessToken>
```

### 3. Get profile

Endpoint:

```text
GET /users/me
```

Expected result:

```text
200 OK
```

### 4. Update profile

Endpoint:

```text
PUT /users/me
```

Request:

```json
{
  "fullName": "Updated Demo User",
  "phone": "+79991111111"
}
```

Expected result:

```text
200 OK
```

### 5. Add address

Endpoint:

```text
POST /users/me/addresses
```

Request:

```json
{
  "city": "Казань",
  "street": "Баумана",
  "house": "10",
  "apartment": "15",
  "entrance": "1",
  "comment": "Домофон 15",
  "isDefault": true
}
```

Expected result:

```text
201 Created
```

### 6. Get addresses

Endpoint:

```text
GET /users/me/addresses
```

Expected result:

```text
200 OK
```

## Full scenario after Catalog and Order are added

1. Register user: `POST /users/register`.
2. Login: `POST /users/login`.
3. Add delivery address: `POST /users/me/addresses`.
4. Open categories: `GET /categories`.
5. Open dishes: `GET /dishes`.
6. Filter dishes: `GET /dishes?categoryId=1`.
7. Search dishes: `GET /dishes?search=чак`.
8. Create order: `POST /orders`.
9. Open order: `GET /orders/{id}`.
10. Open user orders: `GET /orders/my`.
11. Pay order: `POST /orders/{id}/pay`.
12. Cancel order, if the status allows it: `POST /orders/{id}/cancel`.
