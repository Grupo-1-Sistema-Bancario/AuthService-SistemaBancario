# AuthService Sistema Bancario - Grupo 1 IN6BV

Este proyecto es el microservicio de autenticación y gestión de usuarios para el Sistema Bancario. Desarrollado con .NET y C#, se encarga de centralizar la seguridad, emisión de JSON Web Tokens (JWT), validación de correos electrónicos y administración de roles, utilizando PostgreSQL como base de datos.

## Tecnologías Utilizadas

El servicio está construido sobre la plataforma **.NET**. La persistencia de datos relacionales se maneja a través de **PostgreSQL** ejecutado en un contenedor de Docker. Para la gestión de imágenes de perfil se integra **Cloudinary**, y la comunicación por correo electrónico (verificaciones y recuperación de contraseñas) funciona mediante **SMTP**. La seguridad global está respaldada por **JWT** y limitadores de tasa (Rate Limiting) nativos de .NET.

## Instalación y Ejecución

1. Clonar el repositorio en el entorno local.
2. Revisar y ajustar el archivo `appsettings.json` (claves de JWT, Cloudinary, SMTP, base de datos).
3. Levantar el contenedor de la base de datos PostgreSQL ejecutando el comando `docker compose up -d`.
4. Compilar la solución para restaurar las dependencias ejecutando `dotnet build`.
5. Iniciar el servidor ejecutando `dotnet run --project .\src\AuthServiceSistemaBancario.Api\`.

## Configuración Principal (appsettings.json)

El sistema depende de configuraciones críticas para funcionar. La base de datos corre en el puerto 5436 localmente. El secreto del JWT, la configuración de la cuenta de Gmail para envíos SMTP y las credenciales de la API de Cloudinary deben mantenerse sincronizadas con este archivo para que el registro de usuarios y la verificación de correos funcionen sin problemas.

## Rutas Principales (Endpoints)

| Módulo | Método | Endpoint | Descripción |
|---|---|---|---|
| **Health** | GET | `/api/v1/health` | Verifica el estado de salud del microservicio |
| **Auth** | POST | `/api/v1/auth/register` | Registra un usuario nuevo |
| **Auth** | POST | `/api/v1/auth/login` | Autentica al usuario y devuelve un JWT |
| **Auth** | GET | `/api/v1/auth/profile` | Obtiene el perfil del usuario autenticado |
| **Auth** | POST | `/api/v1/auth/verify-email` | Verifica la cuenta de correo mediante un token |
| **Auth** | POST | `/api/v1/auth/resend-verification` | Reenvía el correo de verificación |
| **Auth** | POST | `/api/v1/auth/forgot-password` | Solicita el restablecimiento de contraseña |
| **Auth** | POST | `/api/v1/auth/reset-password` | Cambia la contraseña usando un token de recuperación |
| **Users** | GET | `/api/v1/users/by-role/{roleName}` | (Admin) Obtiene usuarios filtrados por rol |
| **Users** | GET | `/api/v1/users/{userId}/roles` | (Admin) Obtiene los roles de un usuario específico |
| **Users** | PUT | `/api/v1/users/{userId}/role` | (Admin) Actualiza el rol de un usuario |