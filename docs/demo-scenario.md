# Demo scenario

## Цель

Показать основной путь пользователя в сервисе доставки татарской еды.

## Сценарий

1. Пользователь регистрируется через `POST /users/register`.
2. Пользователь входит через `POST /users/login`.
3. Пользователь получает профиль через `GET /users/me`.
4. Пользователь добавляет адрес через `POST /users/me/addresses`.
5. Пользователь смотрит категории через `GET /categories`.
6. Пользователь смотрит блюда через `GET /dishes`.
7. Пользователь фильтрует блюда по категории через `GET /dishes?categoryId=1`.
8. Пользователь ищет блюдо через `GET /dishes?search=%D1%87%D0%B0%D0%BA`.
9. Пользователь создает заказ через `POST /orders`.
10. Пользователь смотрит заказ через `GET /orders/{id}`.
11. Пользователь смотрит свои заказы через `GET /orders/my?userId=1`.
12. Пользователь оплачивает заказ через `POST /orders/{id}/pay`.
13. Доставка переводит заказ в доставленный через `POST /orders/{id}/deliver`.

## Local URLs

- User Service: `http://localhost:5100`
- Catalog Service: `http://localhost:5078`
- Order Service: `http://localhost:5007`
