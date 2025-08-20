# 🏦 IdatBank - Backend (API REST)

**Proyecto final del curso Herramientas de Programación II**

Backend de la aplicación bancaria digital IdatBank, desarrollado en ASP.NET Core, que expone servicios REST para gestionar usuarios, cuentas y transferencias de forma segura.

---

## 🔑 Funcionalidades Principales

-🔐 Autenticación de usuarios: Login con validación de credenciales.

-💳 Gestión de cuentas: Consultar saldo disponible.

-💸 Transferencias internas: Movimiento de dinero entre cuentas del mismo usuario.

---

## 🛠 Tecnologías Utilizadas

### Backend

-⚙️ **ASP.NET Core Web API**

-🗄 **SQL Server (base de datos)**

-🔑 **Entity Framework Core (ORM)**

---

### Herramientas

-📬 Postman (pruebas de API)

-🗂 Git & GitHub (control de versiones)

---

## 🚀 **Instalación y Ejecución**

### Clonar repositorio
```bash
git clone https://github.com/DDR144/-Backend-IdatBank.git
```

### Entrar en la carpeta del proyecto
```bash
cd -Backend-IdatBank
```

### Configurar la base de datos

Abrir el archivo appsettings.json y actualizar la cadena de conexión a SQL Server:

```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=IdatBankDB;User Id=sa;Password=yourpassword;TrustServerCertificate=True"
}
```
