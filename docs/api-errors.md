# API errors

## Common error format

```json
{
  "message": "Validation failed.",
  "errors": {
    "email": [
      "The Email field is required."
    ]
  }
}
```

## Current formats

User Service and Order Service return:

```json
{
  "message": "Order not found.",
  "errors": null
}
```

Catalog Service returns:

```json
{
  "detail": "Dish not found",
  "code": "NOT_FOUND"
}
```

## Status codes

- `400 Bad Request` - invalid request data.
- `401 Unauthorized` - missing or invalid user token.
- `404 Not Found` - requested entity was not found.
- `409 Conflict` - duplicate data, for example existing email.
- `500 Internal Server Error` - unexpected server error.
