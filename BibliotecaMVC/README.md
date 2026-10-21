# BibliotecaMVC – Sistema de Gestión de Biblioteca

## Descripción del proyecto

BibliotecaMVC es una aplicación web desarrollada con ASP.NET Core MVC, Entity Framework Core, SQL Server y ASP.NET Core Identity.

Permite gestionar libros, autores y categorías, así como administrar usuarios mediante autenticación y autorización basada en roles.

## Tecnologías utilizadas

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap
- HTML y CSS

## Requisitos

- Visual Studio con soporte para ASP.NET Core.
- SDK de .NET compatible con el proyecto.
- SQL Server.
- Entity Framework Core Tools.

## Instalación y configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/danie-0101/BibliotecaMVC.git
```

Abrir la solución en Visual Studio.

### 2. Configurar la base de datos

En `BibliotecaMVC/appsettings.json`, configurar `DefaultConnection` según la instancia local de SQL Server.

Ejemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BibliotecaDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Crear o actualizar la base de datos

Abrir la terminal en la carpeta que contiene `BibliotecaMVC.csproj` y ejecutar:

```powershell
dotnet ef database update
```

Este comando aplica las migraciones configuradas. Si el proyecto utiliza contextos separados para la biblioteca e Identity, deben aplicarse las migraciones de cada contexto correspondiente.

### 4. Configurar la cuenta administradora inicial

El proyecto utiliza ASP.NET Core Identity para crear y administrar usuarios y roles.

Desde la carpeta `BibliotecaMVC`, ejecutar:

```powershell
dotnet user-secrets init
dotnet user-secrets set "BootstrapAdmin:Email" "admin@biblioteca.com"
dotnet user-secrets set "BootstrapAdmin:Password" "CONTRASEÑA_SEGURA_ELEGIDA_POR_EL_DOCENTE"
```

Reemplazar el valor de ejemplo por una contraseña segura que cumpla las reglas de Identity.

Al iniciar la aplicación con una base de datos configurada, `Program.cs` creará la cuenta si no existe y le asignará el rol `Admin`.

**Importante:** Si el correo ya existe, el proceso no reemplaza su contraseña. Los secretos son locales y no se incluyen en GitHub.

Después de crear la cuenta, retirar el secreto de contraseña:

```powershell
dotnet user-secrets remove "BootstrapAdmin:Password"
```

### 5. Ejecutar la aplicación

```powershell
dotnet run
```

Abrir la dirección HTTPS indicada en la terminal.

## Roles y permisos

| Operación | Usuario | Bibliotecario | Admin |
|---|---|---|---|
| Consultar libros | Sí | Sí | Sí |
| Agregar libros | No | Sí | Sí |
| Editar libros | No | Sí | Sí |
| Eliminar libros | No | Sí | Sí |

### Administrador

El administrador puede consultar, agregar, editar y eliminar libros, además de administrar los roles de los usuarios.

Para ingresar, utilizar el correo y la contraseña configurados durante la instalación.

### Bibliotecario

Para probar el rol Bibliotecario:

1. Registrar una cuenta desde la aplicación.
2. Cerrar sesión.
3. Iniciar sesión con Admin.
4. Entrar a la sección Roles.
5. Asignar el rol Bibliotecario a la cuenta registrada.
6. Cerrar sesión e iniciar sesión con la cuenta Bibliotecario.

El bibliotecario podrá consultar, agregar, editar y eliminar libros.

### Usuario

Las cuentas registradas normalmente reciben el rol Usuario.

Este rol permite consultar el listado y los detalles de los libros, pero no permite agregar, editar ni eliminar.

## Seguridad

La aplicación implementa:

- Autenticación con ASP.NET Core Identity.
- Autorización mediante `[Authorize(Roles = "...")]`.
- Restricciones en las acciones GET y POST del módulo Libros.
- Protección antifalsificación mediante `[ValidateAntiForgeryToken]`.
- Visualización condicional de botones mediante `User.IsInRole(...)`.
- Restricción de acceso directo a operaciones no autorizadas.

## Pruebas de autorización

Para verificar el funcionamiento:

1. Iniciar sesión como Admin y probar todas las operaciones CRUD de Libros.
2. Iniciar sesión como Bibliotecario y comprobar que puede crear, editar y eliminar.
3. Iniciar sesión como Usuario y comprobar que solamente puede consultar.
4. Intentar acceder a `/Libros/Create` con el rol Usuario y comprobar que se deniega el acceso.
5. Verificar que el inicio y cierre de sesión funcionen.
6. Comprobar que los módulos Autores y Categorías continúen funcionando.

## Consideraciones

No se incluyen contraseñas ni secretos personales en el repositorio. Cada persona que ejecute el proyecto debe configurar su propia conexión a SQL Server y su cuenta administradora inicial.

## Actividad académica

Implementación de administración de roles y autorización de operaciones CRUD del módulo Libros, utilizando ASP.NET Core Identity.

Roles implementados: **Admin, Bibliotecario y Usuario**.