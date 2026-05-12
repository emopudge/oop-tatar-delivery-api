from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, EmailStr
from typing import List, Optional
from datetime import datetime
from enum import Enum

app = FastAPI(
    title="Tatar Food Delivery API",
    description="REST API для сервиса доставки татарской еды (Этап 2)",
    version="1.0.0"
)

# ==================== МОДЕЛИ (DTO) ====================

# User Service
class UserCreate(BaseModel):
    email: EmailStr
    password: str
    name: str
    phone: str

class UserLogin(BaseModel):
    email: EmailStr
    password: str

class AddressCreate(BaseModel):
    city: str
    street: str
    house: str
    apartment: Optional[str] = None
    entrance: Optional[str] = None
    comment: Optional[str] = None
    is_default: bool = False

class UserResponse(BaseModel):
    id: int
    email: str
    name: str
    phone: str
    created_at: datetime

class AddressResponse(BaseModel):
    id: int
    city: str
    street: str
    house: str
    apartment: Optional[str]
    is_default: bool

# Catalog Service
class CategoryResponse(BaseModel):
    id: int
    name: str
    description: Optional[str]

class DishCreate(BaseModel):
    name: str
    description: Optional[str]
    price: float
    category_id: int
    is_available: bool = True

class DishResponse(BaseModel):
    id: int
    name: str
    description: Optional[str]
    price: float
    category_id: int
    is_available: bool

# Order Service
class OrderItemCreate(BaseModel):
    dish_id: int
    quantity: int

class OrderCreate(BaseModel):
    items: List[OrderItemCreate]
    address_id: int

class OrderResponse(BaseModel):
    orderId: int
    status: str
    totalPrice: float

# ==================== ЭНДПОИНТЫ ====================

# User Service
@app.post("/users/register", response_model=UserResponse, tags=["User Service"])
async def register_user(user: UserCreate):
    """Регистрация нового пользователя"""
    # TODO: реализовать логику
    return UserResponse(id=1, email=user.email, name=user.name, phone=user.phone, created_at=datetime.now())

@app.post("/users/login", tags=["User Service"])
async def login_user(credentials: UserLogin):
    """Авторизация пользователя"""
    return {"access_token": "fake-jwt-token", "token_type": "bearer"}

@app.get("/users/me", response_model=UserResponse, tags=["User Service"])
async def get_current_user():
    """Получение текущего пользователя"""
    raise HTTPException(status_code=401, detail="Not authenticated")

@app.post("/users/me/addresses", response_model=AddressResponse, tags=["User Service"])
async def add_address(address: AddressCreate):
    """Добавление адреса пользователя"""
    return AddressResponse(id=1, **address.dict())

@app.get("/users/me/addresses", response_model=List[AddressResponse], tags=["User Service"])
async def get_user_addresses():
    """Получение всех адресов пользователя"""
    return []

# Catalog Service
@app.get("/categories", response_model=List[CategoryResponse], tags=["Catalog Service"])
async def get_categories():
    """Получение всех категорий"""
    return []

@app.get("/dishes", response_model=List[DishResponse], tags=["Catalog Service"])
async def get_dishes(category_id: Optional[int] = None):
    """Получение списка блюд (с фильтрацией по категории)"""
    return []

@app.get("/dishes/{dish_id}", response_model=DishResponse, tags=["Catalog Service"])
async def get_dish(dish_id: int):
    """Получение информации о блюде"""
    raise HTTPException(status_code=404, detail="Dish not found")

@app.post("/dishes", response_model=DishResponse, tags=["Catalog Service"])
async def create_dish(dish: DishCreate):
    """Создание нового блюда (только для админа)"""
    raise HTTPException(status_code=403, detail="Admin only")

# Order Service
@app.post("/orders", response_model=OrderResponse, tags=["Order Service"])
async def create_order(order: OrderCreate):
    """Создание нового заказа"""
    return OrderResponse(orderId=1, status="PendingPayment", totalPrice=1450.0)

@app.get("/orders/{order_id}", response_model=OrderResponse, tags=["Order Service"])
async def get_order(order_id: int):
    """Получение информации о заказе"""
    raise HTTPException(status_code=404, detail="Order not found")

@app.get("/orders/my", response_model=List[OrderResponse], tags=["Order Service"])
async def get_my_orders():
    """Получение моих заказов"""
    return []

@app.post("/orders/{order_id}/cancel", tags=["Order Service"])
async def cancel_order(order_id: int):
    """Отмена заказа"""
    return {"status": "cancelled"}

@app.post("/orders/{order_id}/pay", tags=["Order Service"])
async def pay_order(order_id: int):
    """Оплата заказа"""
    return {"status": "paid"}

@app.post("/orders/{order_id}/deliver", tags=["Order Service"])
async def deliver_order(order_id: int):
    """Завершение доставки (внутренний эндпоинт)"""
    return {"status": "delivered"}

# Запуск: uvicorn main:app --reload