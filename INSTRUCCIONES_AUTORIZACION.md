# BibliotecaMVC — autorización y roles

## Requisitos
Visual Studio con .NET 10, SQL Server/SQL Express y EF Core Tools. Ajustar `DefaultConnection` en `appsettings.json` según el servidor SQL disponible.

## Base de datos y roles
Abrir terminal en la carpeta `BibliotecaMVC` (donde está el `.csproj`) y ejecutar:

```powershell
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add IdentityInicial --context IdentityDbContext --output-dir Migrations/Identity
dotnet ef database update --context ApplicationDbContext
dotnet ef database update --context IdentityDbContext
```

**Importante:** La migración IdentityInicial debe generarse solo una vez. Si `dotnet-ef` ya está instalado, omitir el paso de instalación. La base de datos SQL debe existir o ser accesible por la cuenta configurada.

Crear el administrador inicial con **User Secrets**, sin poner la contraseña en archivos de código fuente:

```powershell
dotnet user-secrets init
dotnet user-secrets set "BootstrapAdmin:Email" "admin@biblioteca.local"
dotnet user-secrets set "BootstrapAdmin:Password" "CambiarEstaClave123!"
dotnet run
```

Cambiar la contraseña de ejemplo antes de ejecutar. **Después del primer inicio**, quitar el secreto `BootstrapAdmin:Password` para evitar que el arranque vuelva a conceder permisos de administrador a ese correo:

```powershell
dotnet user-secrets remove "BootstrapAdmin:Password"
```

En el primer inicio se crean los roles Administrador, Bibliotecario y Lector. Registrar otros usuarios desde `/Cuenta/Registro` (inicialmente Lector). Entrar como administrador y acceder a `/Roles` para asignar Bibliotecario o Administrador. Tras cambiar un rol, el usuario debe cerrar sesión y volver a entrar.

## Pruebas
- Sin iniciar sesión: `/Libros` redirige a `/Cuenta/Login`.
- Lector: puede ver `/Libros` y detalles, pero `/Libros/Create`, `/Libros/Edit/1` y `/Libros/Delete/1` muestran acceso denegado.
- Bibliotecario: puede consultar, crear y editar; eliminar muestra acceso denegado.
- Administrador: puede consultar, crear, editar, eliminar y administrar roles.
- Probar acceso directo por URL, Login y Logout; verificar que Autores y Categorías continúan disponibles.

## Nota
El proyecto no pudo compilarse aquí porque el entorno de generación no dispone del SDK de .NET. Ejecutar `dotnet build` y las pruebas en Visual Studio. Las migraciones Identity se generan en tu equipo para evitar editar a mano las tablas de Identity.
