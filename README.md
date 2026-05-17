# Tatar Food Delivery API

Репозиторий для проекта по ООП: сервис заказа татарской еды с микросервисной архитектурой.

Рабочая реализация `User Service` на C# лежит в `TatarDelivery.UserService`.

## Что реализовано

`User Service` поддерживает:

- `POST /users/register`
- `POST /users/login`
- `GET /users/me`
- `PUT /users/me`
- `GET /users/me/addresses`
- `POST /users/me/addresses`

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

```bash
cd TatarDelivery.UserService
dotnet build
dotnet run
```

Swagger будет доступен по адресу:

- `http://localhost:5100/swagger`

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
