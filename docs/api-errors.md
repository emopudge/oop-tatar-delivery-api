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

## Current User Service format

`User Service` currently returns errors in this format:

```json
{
  "message": "A user with this email already exists.",
  "errors": null
}
```

Validation errors return `message` and `errors` with field names.

## HTTP status codes

- `400 Bad Request` - invalid request data.
- `401 Unauthorized` - user is not authenticated.
- `403 Forbidden` - user has no permission for the action.
- `404 Not Found` - entity was not found.
- `409 Conflict` - data conflict, for example duplicate email.
- `500 Internal Server Error` - unexpected server error.
