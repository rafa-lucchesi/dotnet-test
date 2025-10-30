# Product API

A REST API built with .NET 9.0 featuring JWT authentication, role-based authorization, and CRUD operations.

## Features

- JWT Authentication
- Role-based Authorization (Admin, Manager, User)
- Product Management (CRUD)
- SQLite Database
- Swagger Documentation

## Requirements

- .NET 9.0 SDK

## Setup

```bash
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5000`

## Default Credentials

- Username: `admin`
- Password: `admin123`

## Endpoints

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login

### Products (Requires Authentication)

- `GET /api/products` - List products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /api/products/statistics` - Get statistics (Admin only)

## Authorization

- **Admin**: Full access to all resources
- **Manager**: Can view all products, manage own products
- **User**: Can only manage own products

## Example Usage

### Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

### Create Product

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"name":"Product","description":"Description","price":100,"quantity":10}'
```

## License

MIT
