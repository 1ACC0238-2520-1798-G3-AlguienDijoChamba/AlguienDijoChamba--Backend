﻿# AlguienDijoChamba - Backend API

Este repositorio contiene el código fuente para el backend de la aplicación **AlguienDijoChamba**. La API está construida con **.NET 9** siguiendo los principios de **Domain-Driven Design (DDD)** y **CQRS**, y utiliza **MySQL** como base de datos.

## \#\# Características ✨

* **Arquitectura DDD:** Organizado por dominios de negocio (`IAM`, `Professionals`, `Jobs`).
* **Autenticación JWT:** Sistema de registro y login seguro basado en JSON Web Tokens.
* **CQRS con MediatR:** Separación de comandos (escritura) y queries (lectura) para una lógica de aplicación más limpia.
* **Entity Framework Core:** ORM para la interacción con la base de datos MySQL.
* **Integración de API Externa:** Conexión con la API de RENIEC para validación de DNI.
* **Documentación con Swagger:** Interfaz de API interactiva y autogenerada.

-----

## \#\# Guía de Instalación ⚙️

Sigue estos pasos para configurar el proyecto en tu máquina local.

### 1\. Clonar el Repositorio

Abre una terminal y clona el proyecto:

```shell
git clone https://github.com/1ACC0238-2520-1798-G3-AlguienDijoChamba/AlguienDijoChamba--Backend.git
cd AlguienDijoChamba
```

### 2\. Configurar las Variables de Entorno

Este proyecto utiliza un archivo `.env` para manejar las claves y cadenas de conexión, el cual no se sube al repositorio.

1.  En la **raíz de la solución**, crea un nuevo archivo llamado **`.env`**.

2.  Copia y pega el siguiente contenido, **ajustando la contraseña de tu base de datos MySQL**:

    ```dotenv
    DB_CONNECTION_STRING="server=localhost;port=3306;database=chamba_db;user=root;password=TU_CONTRASEÑA_SECRETA_DE_MYSQL"
    RENIEC_API_KEY=""
    JWT_SECRET_KEY="una_clave_secreta_muy_larga_y_segura_para_firmar_tokens_jwt_de_32_caracteres_o_mas"
    JWT_ISSUER="AlguienDijoChambaAPI"
    JWT_AUDIENCE="AlguienDijoChambaApp"
    ```

### 3\. Configurar la Base de Datos

1.  **Crea la base de datos** en tu servidor MySQL. Puedes usar un cliente de base de datos como DBeaver o HeidiSQL y ejecutar:
    ```sql
    CREATE DATABASE chamba_db;
    ```
2.  **Instala las herramientas de Entity Framework Core** (si no las tienes):
    ```shell
    dotnet tool install --global dotnet-ef
    ```
3.  **Aplica las migraciones.** Abre una terminal en la carpeta del proyecto `AlguienDijoChamba.Api` y ejecuta el siguiente comando para crear todas las tablas:
    ```shell
    dotnet ef database update
    ```

-----

## \#\# Ejecutar la Aplicación 🚀

Una vez configurado el proyecto, puedes ejecutarlo de dos maneras:

* **Desde Rider o Visual Studio:**
  Simplemente presiona el botón de **"Run"** (▶️).

* **Desde la terminal:**
  Navega a la carpeta del proyecto `AlguienDijoChamba.Api` y ejecuta:

  ```shell
  dotnet run
  ```

La API se iniciará y estará disponible en `http://localhost:5101` (el puerto puede variar). La documentación de Swagger se abrirá automáticamente en tu navegador.

-----

## \#\# Migraciones de la Base de Datos 🗄️

### Aplicar Migraciones Existentes

**IMPORTANTE:** Antes de ejecutar la aplicación por primera vez, debes aplicar las migraciones para crear las tablas en la base de datos.

**Método 1 - Script Automático (Recomendado):**
```shell
# Windows CMD
ejecutar-migracion.bat

# PowerShell
.\aplicar-migracion.ps1
```

**Método 2 - Comandos Manuales:**
```shell
# 1. Compila el proyecto
dotnet build

# 2. Aplica todas las migraciones pendientes
dotnet ef database update
```

**Método 3 - SQL Directo:**
Si prefieres, puedes ejecutar manualmente el script `add_missing_columns.sql` en tu cliente MySQL.

### Verificar Migraciones Aplicadas

Para ver qué migraciones ya están aplicadas:
```shell
dotnet ef migrations list
```

### Crear Nuevas Migraciones

Si realizas cambios en las entidades del dominio (en las carpetas `Domain`), necesitarás crear una nueva migración para actualizar la base de datos.

1.  Abre una terminal en la carpeta del proyecto `AlguienDijoChamba.Api`.

2.  Ejecuta el siguiente comando, reemplazando `NombreDeLaMigracion` con una descripción de tus cambios (ej: `AddSpecialtiesTable`):

    ```shell
    dotnet ef migrations add NombreDeLaMigracion -o src/Shared/Infrastructure/Persistence/EFC/Migrations
    ```

3.  Aplica la nueva migración a la base de datos:

    ```shell
    dotnet ef database update
    ```

### Scripts Disponibles

El proyecto incluye varios scripts de ayuda:

- **`ejecutar-migracion.bat`** - Script CMD que compila y aplica migraciones
- **`aplicar-migracion.ps1`** - Script PowerShell con validación completa
- **`add_missing_columns.sql`** - Script SQL para aplicación manual
- **`SOLUCION_MIGRACION.md`** - Documentación detallada de la migración actual
