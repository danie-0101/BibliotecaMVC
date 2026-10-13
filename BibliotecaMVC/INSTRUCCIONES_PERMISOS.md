# Control de acceso actualizado

- Administrador: acceso completo a Libros, Autores, Categorías y administración de roles.
- Lector: consulta de Libros, Autores y Categorías. No puede crear, editar ni eliminar.
- Máximo dos administradores: el panel Roles impide ascender a un tercero.
- El registro público asigna Lector.
- Para habilitar un segundo administrador, ingrese como administrador en /Roles y asígnele el rol.
- Si ya existen más de dos administradores en la base de datos, revise y reduzca sus roles manualmente desde una cuenta administradora; el límite se aplica a nuevas asignaciones.
- Los usuarios deben cerrar sesión y volver a entrar tras cambiar de rol.
- Verifique con pruebas de acceso directo a las rutas /Autores/Edit/1, /Autores/Delete/1, /Categorias/Create, /Categorias/Edit/1 y /Libros/Create.
- No se requiere una migración adicional si las tablas Identity ya existen.