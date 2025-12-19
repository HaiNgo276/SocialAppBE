# ?? H??ng D?n Deploy SocialNetworkBe

## ?? L?u � Quan Tr?ng

**Vercel KH�NG h? tr? .NET Backend!** Vercel ch? h? tr?:
- Frontend (React, Next.js, Vue)
- Serverless Functions (Node.js, Python, Go)

## ?? C�c Bi?n M�i Tr??ng C?n Thi?t (.env)

| Bi?n | M� t? | V� d? |
|------|-------|-------|
| `ConnectionStrings__MyDb` | SQL Server connection string | `Server=tcp:xxx.database.windows.net;...` || `Cassandra__ContactPoint` | DataStax Astra contact point | `xxx-us-east-2.db.astra.datastax.com` |
| `Cassandra__Token` | Astra application token | `AstraCS:clientId:token...` |
| `Cassandra__Keyspace` | Cassandra keyspace name | `fricon` || `JWT__Key` | Secret key cho JWT (min 32 chars) | `7b1b14d2e8e34d9c9f6a9a7a35b5f2d1...` |
| `JWT__Issuer` | JWT Issuer URL | `https://your-api.com` |
| `JWT__Audience` | JWT Audience URL | `https://your-api.com` |
| `Cloudinary__Api` | Cloudinary API URL | `cloudinary://key:secret@cloud` |
| `AllowedOrigins` | CORS origins (comma separated) | `https://frontend.vercel.app` |

---

## ?? Option 1: Deploy l�n Railway (Recommended - Free)

### B??c 1: Chu?n b? GitHub
```bash
# ??m b?o Dockerfile ? root directory
git add Dockerfile .dockerignore .env.example
git commit -m "Add Docker support for deployment"
git push origin main
```

### B??c 2: Deploy tr�n Railway
1. Truy c?p [railway.app](https://railway.app)
2. ??ng nh?p b?ng GitHub
3. Click **"New Project"** ? **"Deploy from GitHub repo"**
4. Ch?n repo `SocialNetworkBe`
5. Railway s? t? detect Dockerfile

### B??c 3: Th�m Environment Variables
1. Click v�o service v?a t?o
2. V�o tab **"Variables"**
3. Th�m c�c bi?n:
```
ConnectionStrings__MyDb=Server=tcp:ngohai.database.windows.net,1433;Initial Catalog=SocialNetwork;User ID=admin123;Password=ngohai2706@;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
Cassandra__ContactPoint=f2a67376-7a04-4bcd-9f82-0b19c958647b-us-east-2.db.astra.datastax.com
Cassandra__Token=AstraCS:YOUR_CLIENT_ID:YOUR_TOKEN_HERE
Cassandra__Keyspace=fricon
JWT__Key=7b1b14d2e8e34d9c9f6a9a7a35b5f2d1d6c8a2b7f3e4c9a8b1d5f7c6a3b2e1f0
JWT__Issuer=https://your-app.up.railway.app
JWT__Audience=https://your-app.up.railway.app
Cloudinary__Api=cloudinary://434244532219775:jf0OWJdWHU9Nt1oqG1IjQVyiBlQ@dhnjbohwa
AllowedOrigins=https://your-frontend.vercel.app,http://localhost:3000
ASPNETCORE_ENVIRONMENT=Production
```

### B??c 4: Th�m Database (Optional - n?u c?n SQL Server)
- Railway c� PostgreSQL mi?n ph�
- Ho?c s? d?ng **Azure SQL Free Tier**

---

## ?? Option 2: Deploy l�n Render (Free)

### B??c 1: Chu?n b?
Gi?ng Railway, push code l�n GitHub

### B??c 2: Deploy
1. Truy c?p [render.com](https://render.com)
2. **New** ? **Web Service**
3. Connect GitHub repo
4. Ch?n **Docker** runtime
5. Th�m Environment Variables

### render.yaml (Optional - Infrastructure as Code)
```yaml
services:
  - type: web
    name: socialnetwork-api
    env: docker
    plan: free
    healthCheckPath: /health
    envVars:
      - key: ConnectionStrings__MyDb
        sync: false
      - key: JWT__Key
        sync: false
      - key: JWT__Issuer
        sync: false
      - key: JWT__Audience
        sync: false
      - key: Cloudinary__Api
        sync: false
      - key: AllowedOrigins
        sync: false
```

---

## ?? Option 3: Deploy l�n Azure (T?t nh?t cho .NET)

### S? d?ng Azure CLI
```bash
# Login
az login

# T?o Resource Group
az group create --name SocialNetworkRG --location "Southeast Asia"

# T?o App Service Plan (Free tier)
az appservice plan create --name SocialNetworkPlan --resource-group SocialNetworkRG --sku F1 --is-linux

# T?o Web App
az webapp create --resource-group SocialNetworkRG --plan SocialNetworkPlan --name socialnetwork-api --runtime "DOTNETCORE:8.0"

# Deploy t? GitHub
az webapp deployment source config --name socialnetwork-api --resource-group SocialNetworkRG --repo-url https://github.com/Dragon336699/SocialNetworkBe --branch main --manual-integration

# Ho?c deploy container
az webapp create --resource-group SocialNetworkRG --plan SocialNetworkPlan --name socialnetwork-api --deployment-container-image-name your-docker-image
```

### Th�m Environment Variables tr�n Azure
```bash
az webapp config appsettings set --resource-group SocialNetworkRG --name socialnetwork-api --settings \
  ConnectionStrings__MyDb="your-connection-string" \
  JWT__Key="your-jwt-key" \
  JWT__Issuer="https://socialnetwork-api.azurewebsites.net" \
  JWT__Audience="https://socialnetwork-api.azurewebsites.net" \
  Cloudinary__Api="your-cloudinary-url" \
  AllowedOrigins="https://your-frontend.vercel.app"
```

---

## ?? Option 4: Deploy l�n Fly.io (Free)

### B??c 1: C�i ??t Fly CLI
```bash
# Windows (PowerShell)
powershell -Command "iwr https://fly.io/install.ps1 -useb | iex"

# Login
fly auth login
```

### B??c 2: T?o fly.toml
```toml
app = "socialnetwork-api"
primary_region = "sin"

[build]
  dockerfile = "Dockerfile"

[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = true
  auto_start_machines = true
  min_machines_running = 0

[checks]
  [checks.health]
    port = 8080
    type = "http"
    interval = "30s"
    timeout = "5s"
    path = "/health"
```

### B??c 3: Deploy
```bash
fly launch
fly secrets set ConnectionStrings__MyDb="your-connection-string"
fly secrets set JWT__Key="your-jwt-key"
fly secrets set JWT__Issuer="https://socialnetwork-api.fly.dev"
fly secrets set JWT__Audience="https://socialnetwork-api.fly.dev"
fly secrets set Cloudinary__Api="your-cloudinary-url"
fly secrets set AllowedOrigins="https://your-frontend.vercel.app"
fly deploy
```

---

## ??? Database Options (SQL Server)

### 1. Azure SQL (Recommended)
- Free tier: 32GB storage
- T?o t?i: [portal.azure.com](https://portal.azure.com)
- Connection string format:
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=SocialNetwork;Persist Security Info=False;User ID=your-user;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### 2. PlanetScale (MySQL - c?n chuy?n ??i)
### 3. Supabase (PostgreSQL - c?n chuy?n ??i)
### 4. ElephantSQL (PostgreSQL - c?n chuy?n ??i)

---

## ?? B?o M?t Khi Deploy

1. **KH�NG commit file `.env` l�n GitHub**
2. **??i JWT Key** - t?o key m?i cho production
3. **T?o Cloudinary account m?i** cho production
4. **S? d?ng HTTPS** cho t?t c? endpoints
5. **C?p nh?t AllowedOrigins** v?i domain frontend th?c t?

---

## ?? Checklist Tr??c Khi Deploy

- [ ] ?� push code l�n GitHub
- [ ] Dockerfile ? th? m?c root
- [ ] ?� t?o database tr�n cloud (Azure SQL)
- [ ] ?� c� Cloudinary account
- [ ] ?� generate JWT Key m?i cho production
- [ ] ?� config AllowedOrigins v?i frontend URL
- [ ] ?� test build local v?i Docker:
  ```bash
  docker build -t socialnetwork-api .
  docker run -p 8080:8080 --env-file .env socialnetwork-api
  ```

---

## ?? Troubleshooting

### L?i Database Connection
- Ki?m tra firewall rules c?a SQL Server
- ??m b?o IP c?a server ???c whitelist

### L?i CORS
- Ki?m tra `AllowedOrigins` c� ?�ng URL frontend kh�ng
- ??m b?o kh�ng c� trailing slash

### L?i JWT
- Ki?m tra JWT Key ?? d�i (min 32 characters)
- Issuer/Audience ph?i match v?i domain API

### SignalR kh�ng ho?t ??ng
- M?t s? free tier kh�ng h? tr? WebSocket
- C?n upgrade plan ho?c d�ng Long Polling fallback
