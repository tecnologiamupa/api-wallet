# Google Wallet API - Pases Genéricos

API REST desarrollada en .NET para la integración con Google Wallet API, permitiendo la creación y actualización de pases genéricos (Generic Passes).

## 🚀 Características

- Creación de pases genéricos de Google Wallet
- Actualización de pases existentes
- Firma JWT personalizada para autenticación con Google
- Configuración flexible mediante appsettings.json
- Validación de CORS configurable

## 📋 Requisitos Previos

- .NET 6.0 o superior
- Una cuenta de Google Cloud Platform
- Google Wallet API habilitada
- Cuenta de servicio de Google con permisos de Google Wallet API

## ⚙️ Configuración

### 1. Configurar Google Cloud Platform

1. Accede a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un nuevo proyecto o selecciona uno existente
3. Habilita la Google Wallet API
4. Crea un Issuer ID en la [consola de Google Wallet](https://pay.google.com/business/console)
5. Crea una cuenta de servicio:
   - Ve a IAM & Admin > Service Accounts
   - Crea una nueva cuenta de servicio
   - Descarga el archivo JSON de credenciales

### 2. Configurar la Aplicación

Edita el archivo `appsettings.json` con tus credenciales:

```json
{
  "Wallet": {
    "IssuerId": "TU_ISSUER_ID",
    "ClassId": "TU_CLASS_ID",
    "Origins": [ 
      "http://localhost:4200",
      "https://tudominio.com"
    ],
    "ServiceAccountJsonPath": "ruta/a/tu/service-account.json"
  }
}
```

### 3. Archivo de Credenciales

Coloca tu archivo `service-account.json` en una ubicación segura y actualiza la ruta en `appsettings.json`.

**⚠️ IMPORTANTE:** Nunca subas el archivo `service-account.json` a repositorios públicos.

## 🔧 Instalación

```bash
# Clonar el repositorio
git clone <tu-repositorio>

# Navegar al directorio
cd apiwallet

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build
```

## ▶️ Ejecución

```bash
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## 📡 Endpoints

### Crear un Pase

**POST** `/api/wallet/generic-pass`

```json
{
  "objectId": "identificador-unico",
  "cardTitle": "Título del Pase",
  "header": "Encabezado",
  "subheader": "Subencabezado",
  "body": "Contenido principal",
  "footer": "Pie de página",
  "hexBackgroundColor": "#4285F4",
  "logoImageUri": "https://ejemplo.com/logo.png",
  "heroImageUri": "https://ejemplo.com/hero.png"
}
```

**Respuesta:**
```json
{
  "saveUrl": "https://pay.google.com/gp/v/save/..."
}
```

### Actualizar un Pase

**PUT** `/api/wallet/generic-pass/{objectId}`

```json
{
  "cardTitle": "Título Actualizado",
  "header": "Nuevo Encabezado",
  "body": "Contenido actualizado"
}
```

## 🏗️ Estructura del Proyecto

```
WalletGoogle/
├── Models/
│   ├── CreatePassRequest.cs      # Modelo para crear pases
│   ├── UpdatePassRequest.cs      # Modelo para actualizar pases
│   └── WalletOptions.cs          # Configuración de Wallet
├── Services/
│   ├── GoogleWalletService.cs    # Lógica principal de integración
│   ├── JwtSigner.cs              # Firma de tokens JWT
│   └── TimeHelpers.cs            # Utilidades de tiempo
├── Program.cs                     # Punto de entrada de la aplicación
├── appsettings.json              # Configuración de la aplicación
└── WalletGoogle.http             # Ejemplos de peticiones HTTP
```

## 🔐 Seguridad

- Las credenciales deben mantenerse en archivos locales o servicios de gestión de secretos
- Configura CORS apropiadamente para tus dominios autorizados
- Nunca expongas tu `IssuerId` o credenciales en código público
- Usa HTTPS en producción

## 📚 Recursos Adicionales

- [Google Wallet API Documentation](https://developers.google.com/wallet)
- [Generic Pass Documentation](https://developers.google.com/wallet/generic)
- [Google Cloud Console](https://console.cloud.google.com/)
- [Google Pay & Wallet Console](https://pay.google.com/business/console)

## 🤝 Contribución

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -m 'Añadir nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

## 📝 Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo `LICENSE` para más detalles.

## ✨ Autor

Desarrollado para la integración con Google Wallet API.
