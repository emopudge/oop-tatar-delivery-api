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

----------------------------------------------------------------

Вот очищенная версия:

---

# 🥟Frontend: Инструкция по запуску

## Предварительные требования

- Docker Desktop (для PostgreSQL)
- .NET 8 SDK
- Visual Studio Code (с расширениями C#, C# Dev Kit)
- Git

## Пошаговый запуск

### Шаг 1: Запусти Docker Desktop
Открой Docker Desktop и дождись статуса "Running". PostgreSQL будет запущен автоматически при старте CatalogService.

### Шаг 2: Открой проект в VS Code
```powershell
cd C:\Users\ТвоёИмя\oop-tatar-delivery-api
code .
```

### Шаг 3: Открой 4 вкладки терминала
Нажми `Ctrl+Shift+`` ` `` 4 раза. В каждой вкладке выполни соответствующую команду:

| Вкладка | Команда | Что запускает |
|---------|---------|--------------|
| Catalog | `dotnet run --project TatarDelivery.CatalogService` | Каталог блюд + PostgreSQL в Docker |
| Order | `dotnet run --project TatarDelivery.OrderService` | Заказы + оплата (SQLite) |
| User | `dotnet run --project TatarDelivery.UserService` | Регистрация, вход, профили (SQLite) |
| Delivery | `dotnet run --project TatarDelivery.DeliveryService` | Проверка зоны доставки (в памяти) |

Если порт занят, измени его в `launchSettings.json` соответствующего сервиса.

<img width="1259" height="888" alt="image" src="https://github.com/user-attachments/assets/8377f268-bc3a-4baf-88d4-c4d4e20788dc" />


### Шаг 4: Настрой базу данных для CatalogService (только первый раз)

Подключение к PostgreSQL в Docker (опционально, для отладки):
```powershell
docker exec -it tatar-delivery-catalog-db psql -U postgres -d tatar_delivery_catalog

# Посмотреть блюда:
SELECT "Name", "Price", "ImageUrl" FROM "Dishes";

# Выйти:
\q
```

Seed-код в `TatarDelivery.CatalogService/Program.cs` выполнится автоматически при первом запуске, если таблица `Dishes` пуста.

Если менял модель `Dish.cs`:
```powershell
cd TatarDelivery.CatalogService
dotnet ef migrations add НазваниеИзменения
dotnet ef database update
```

### Шаг 5: Запусти фронтенд

**Способ А: Простой**
1. В проводнике перейди в папку `frontend/`
2. Дважды кликни по `index.html`
3. Страница откроется по адресу `file:///...`

**Способ Б: Live Server**
1. Установи расширение Live Server в VS Code
2. Кликни правой кнопкой по `frontend/index.html` → Open with Live Server
3. Страница откроется на `http://127.0.0.1:5500` с авто-обновлением

### Шаг 6: Проверь работоспособность

- Каталог блюд: открой `http://localhost:5078/swagger` → `GET /dishes` (должен вернуть JSON со списком блюд)
- Авторизация: на сайте нажми «Войти» → зарегистрируйся (в шапке появится имя пользователя)
- Карта: прокрути вниз (маркеры ресторанов и зоны доставки)
- Корзина: добавь блюдо → открой корзину (счётчик обновился, сумма посчитана)
- Доставка: введи `55.796`, `49.108` → «Проверить доставку» (должно быть "Доставка доступна")
- Оплата: нажми «Оформить заказ» (должен создаться заказ с номером)

## Частые проблемы и решения

**CatalogService не подключается к базе**
```
Npgsql.PostgresException: 28P01: password authentication failed
```
Решение:
1. Убедись, что Docker Desktop запущен
2. Проверь строку подключения в `appsettings.Development.json`:
   ```json
   "DefaultConnection": "Host=localhost;Port=5434;Database=tatar_delivery_catalog;Username=postgres;Password=123"
   ```
3. Перезапусти контейнер:
   ```powershell
   docker compose down catalog-db
   docker compose up -d catalog-db
   ```

**Заказ не создаётся: 500 error**
```
SQLite Error 1: 'table Orders has no column named PaymentId'
```
Решение: устарела база OrderService.
```powershell
Remove-Item .\TatarDelivery.OrderService\*.db -Force
dotnet run --project TatarDelivery.OrderService
```

**Картинки блюд не грузятся**

| Причина | Как проверить | Решение |
|---------|--------------|---------|
| Хотлинкинг заблокирован | Открой ссылку из `ImageUrl` в новой вкладке | Используй Unsplash или загрузи картинки на свой сервер |
| Ошибка в JS | F12 → Console | Проверь `renderDishes()`: должно быть `dish.imageUrl`, а не `dish.image` |
| CORS | F12 → Network | В `Program.cs` каждого сервиса добавь `AllowAnyOrigin()` |

**Кнопка "Оформить заказ" неактивна**
Проверь:
- Авторизован ли пользователь
- Есть ли товары в корзине
- Проверена ли зона доставки

## Тестовый сценарий для демонстрации

1. Запусти все 4 сервиса и фронтенд
2. Зарегистрируйся: test@tatar.dev / Test123!
3. Добавь в корзину: Эчпочмак и Чак-чак
4. Проверь доставку: 55.796, 49.108 (центр Казани)
5. Нажми "Оформить заказ"
6. Увидь: "Заказ #1 оформлен и оплачен!"
7. Покажи карту с маркерами ресторанов

## Структура фронтенда

```
frontend/
├── index.html          # Единственный HTML-файл
├── css/
│   └── styles.css      # Стили (опционально)
└── js/
    └── app.js          # Логика (опционально)
```

Для учебного проекта один файл удобен: не нужно настраивать сборку. В продакшене рекомендуется разбивать на модули.

## Обновление проекта

```powershell
# 1. Останови сервисы (Ctrl+C в каждой вкладке)
# 2. Забери изменения:
git pull origin main
# 3. Пересобери:
dotnet restore
# 4. Запусти заново (шаги 3-5)
```

## Нужна помощь?

1. Смотри логи в терминале каждого сервиса
2. Проверяй API через Swagger (`/swagger`)
3. Консоль браузера: F12 → Console
4. Прикрепи скриншот ошибки и шаги, которые привели к ней
