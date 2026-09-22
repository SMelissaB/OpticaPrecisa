# OpticaPrecisa 👓

Sistema de gestión para óptica desarrollado en .NET 8, estructurado bajo una arquitectura limpia en capas para garantizar la escalabilidad, el mantenimiento y la separación de responsabilidades.

## 🏛️ Arquitectura del Proyecto

La solución está dividida en 4 capas principales:

1. **`OpticaPrecisa` (Capa de Presentación / Web):** Aplicación ASP.NET Core MVC encargada de la interfaz de usuario, controladores y vistas.
2. **`CapaNegocio` (Capa de Lógica de Negocio):** Contiene las reglas del negocio, validaciones y servicios intermedios entre la presentación y los datos.
3. **`CapaDatos` (Capa de Acceso a Datos):** Gestiona la conexión con la base de datos utilizando Entity Framework Core (`ApplicationDbContext`).
4. **`CapaEntidad` (Capa de Modelos / Entidades):** Contiene las clases de dominio que representan las tablas de la base de datos (por ejemplo: `Categoria`, `Cliente`, `Producto`, `Venta`, etc.).

## 🛠️ Tecnologías y Herramientas
* **Plataforma:** .NET 8
* **Base de Datos:** SQL Server
* **ORM:** Entity Framework Core (con enfoque *Database First* y estrategia de scaffolding)
* **Control de versiones:** Git

## 🚀 Configuración y Ejecución

1. Clona el repositorio en tu equipo.
2. Abre la solución `OpticaPrecisa.sln` en Visual Studio.
3. Configura tu cadena de conexión en el archivo `appsettings.json` del proyecto web (`OpticaPrecisa`).
4. Asegúrate de que los proyectos apunten a .NET 8 y compila la solución para restaurar los paquetes NuGet necesarios.
5. Ejecuta el proyecto seleccionando `OpticaPrecisa` como proyecto de inicio.