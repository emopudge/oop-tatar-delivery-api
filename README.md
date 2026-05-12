# 🍲 Tatar Food Delivery API

REST API для сервиса доставки национальной татарской кухни.  
Проект в рамках курса ООП, ИТМО, 2026.

## Быстрый старт

### 1. Клонируй репозиторий
```bash
git clone https://github.com/your-username/oop-tatar-delivery-api.git
cd oop-tatar-delivery-api
```
### 2. Создай виртуальное окружение
```bash
# Windows (PowerShell)
python -m venv venv
.\venv\Scripts\activate

# Mac/Linux
python3 -m venv venv
source venv/bin/activate
```
### 3. Установи зависимости
```bash
pip install -r requirements.txt
```
### 4. Запусти сервер
```bash
uvicorn main:app --reload
```
### 5. Открой документацию
- Swagger UI: http://127.0.0.1:8000/docs (интерактивная документация)
- ReDoc: http://127.0.0.1:8000/redoc (альтернативная читаемая документация, если Swagger не загрузится)
- OpenAPI JSON: http://127.0.0.1:8000/openapi.json (Показывает, что спецификация машиночитаемая (для генерации кода))

## Ресурсы
[FastAPI Docs](https://fastapi.tiangolo.com/?spm=a2ty_o01.29997173.0.0.444355fbOXIdIv)

[Swagger/OpenAPI Spec](https://swagger.io/specification/?spm=a2ty_o01.29997173.0.0.444355fbOXIdIv)

[Pydantic Models](https://docs.pydantic.dev/?spm=a2ty_o01.29997173.0.0.444355fbOXIdIv)