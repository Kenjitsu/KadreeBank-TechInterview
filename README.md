# KadreeBank

Solución al **caso Kadree Bank** (sección 3 de la prueba técnica): una API en .NET para gestionar clientes, cuentas, consignaciones, retiros, extractos y reportes, y un frontend en Angular que la consume.

## Resumen

Kadree Bank ofrece dos tipos de cuenta: **ahorros** para personas naturales y **corriente** para empresas. Sobre cada cuenta se pueden hacer consignaciones y retiros.

La solución permite:

- Registrar clientes e ingresar con número de documento y un PIN de 4 dígitos, de esta forma simulando un ingreso a plataforma bancaria.
- Abrir cuentas, consultar el saldo y hacer consignaciones y retiros en la misma cuenta.
- Consultar los movimientos recientes y generar el extracto mensual de una cuenta.
- Generar reportes en tiempo real:
  - Clientes ordenados por número de transacciones en un mes.
  - Clientes cuyos retiros fuera de la ciudad de origen de la cuenta suman más de $1.000.000.

Reglas de negocio principales:

- Una cuenta nunca puede quedar con saldo negativo.
- El saldo es consistente aunque haya consignaciones y retiros simultáneos: cada operación actualiza el saldo con un único `UPDATE` atómico en la base de datos.

## Stack

| Capa | Tecnologías |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, Entity Framework Core 10, SQL Server |
| Frontend | Angular 22, TypeScript, Tailwind CSS 4 |
| Documentación de la API | OpenAPI + Swagger UI |

## Estructura

```text
KadreeBank/
├── API/KadreeBank.API/   ← API
├── client/               ← Frontend en Angular
└── KadreeBank.slnx       ← Solución de .NET
```

## Cómo ejecutar

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server **LocalDB**
- [Node.js v26.10.0](https://nodejs.org/) y npm

### 1. API

Desde la raíz del proyecto:

```bash
dotnet run --project API/KadreeBank.API --launch-profile http
```

- La API queda en `http://localhost:5078`.
- Swagger UI: `http://localhost:5078/swagger`.
- Al iniciar, la API **crea la base de datos `KadreeBankDb`, aplica las migraciones y carga datos de prueba** automáticamente. No hay que ejecutar nada más.
- También se puede ejecutar desde Visual Studio con el perfil `http` o `https`.

**¿Usar otra instancia de SQL Server?** Por defecto se usa `(localdb)\MSSQLLocalDB`. Para usar otra, definir la variable de entorno `ConnectionStrings:KadreeBank`, por ejemplo:

```bash
ConnectionStrings:KadreeBank="Server=.\MI_INSTANCIA;Database=KadreeBankDb;Trusted_Connection=True;TrustServerCertificate=True"
```

### 2. Frontend

Con la API en ejecución, en otra terminaln dirigirse a la carpeta **client** e instalar dependencias y ejecutar el proyecto:

```bash
cd client
npm install
npm start
```
tambien se puede ejecutar la aplicación de angular con el comando:

```bash
ng serve
```

Abre `http://localhost:4200`.

El frontend apunta a `http://localhost:5078/api`. Si la API corre en otra dirección, cambiarla en `client/src/app/core/api.config.ts`.

## Datos de prueba

Al iniciar la API por primera vez se cargan estos clientes de prueba (archivo `API/KadreeBank.API/Data/seedData.json`):

| Cliente | Tipo | Documento | PIN |
|---|---|---|---|
| Laura Gómez | Persona natural | 1012345678 | 1234 |
| Carlos Rodríguez | Persona natural | 79876543 | 5678 |
| María Fernanda López | Persona natural (2 cuentas) | 52345678 | 2468 |
| Inversiones Andinas S.A.S. | Empresa | 900123456 | 1357 |
| Comercializadora del Caribe Ltda. | Empresa | 800987654 | 9753 |
| Andrés Martínez | Persona natural (sin cuentas) | 1098765432 | 1111 |

Incluyen movimientos entre agosto y octubre de 2026 para probar extractos y reportes.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/Auth/login` | Ingreso con documento y PIN |
| POST | `/api/Customers` | Crear cliente |
| GET | `/api/Customers/{id}` | Cliente con sus cuentas |
| POST | `/api/Accounts` | Abrir cuenta |
| GET | `/api/Accounts/{id}` | Datos de la cuenta |
| GET | `/api/Accounts/{id}/balance` | Saldo |
| POST | `/api/Accounts/{id}/deposits` | Consignar |
| POST | `/api/Accounts/{id}/withdrawals` | Retirar |
| GET | `/api/Accounts/{id}/transactions?take=10` | Movimientos recientes |
| GET | `/api/Accounts/{id}/statements/{year}/{month}` | Extracto mensual |
| GET | `/api/Reports/monthly-transactions?year=&month=` | Clientes por número de transacciones en un mes |
| GET | `/api/Reports/out-of-city-withdrawals?year=&month=` | Retiros fuera de la ciudad mayores a $1.000.000 (filtros opcionales) |

En `API/KadreeBank.API/KadreeBank.API.http` hay una petición de ejemplo para cada endpoint.
