# ?? H??ng D?n Deploy SocialNetworkBe

## ?? L?u Ý Quan Tr?ng

**Vercel KHÔNG h? tr? .NET Backend!** Vercel ch? h? tr?:
- Frontend (React, Next.js, Vue)
- Serverless Functions (Node.js, Python, Go)

## ?? Các Bi?n Môi Tr??ng C?n Thi?t (.env)

| Bi?n | Mô t? | Ví d? |
|------|-------|-------|
| `ConnectionStrings__MyDb` | SQL Server connection string | `Server=tcp:xxx.database.windows.net;...` |
| `JWT__Key` | Secret key cho JWT (min 32 chars) | `7b1b14d2e8e34d9c9f6a9a7a35b5f2d1...` |
| `JWT__Issuer` | JWT Issuer URL | `https://your-api.com` |
| `JWT__Audience` | JWT Audience URL | `https://your-api.com` |
| `Cloudinary__Api` | Cloudinary API URL | `cloudinary://key:secret@cloud` |
| `AllowedOrigins` | CORS origins (comma separated) | `https://frontend.vercel.app` |

---

## ?? Option 1: Deploy lên Railway (Recommended - Free)

### B??c 1: Chu?n b? GitHub
```bash
# ??m b?o Dockerfile ? root directory
git add Dockerfile .dockerignore .env.example
git commit -m "Add Docker support for deployment"
git push origin main
```

### B??c 2: Deploy trên Railway
1. Truy c?p [railway.app](https://railway.app)
2. ??ng nh?p b?ng GitHub
3. Click **"New Project"** ? **"Deploy from GitHub repo"**
4. Ch?n repo `SocialNetworkBe`
5. Railway s? t? detect Dockerfile

### B??c 3: Thêm Environment Variables
1. Click vào service v?a t?o
2. Vào tab **"Variables"**
3. Thêm các bi?n:
```
ConnectionStrings__MyDb=Server=tcp:your-server.database.windows.net,1433;Database=SocialNetwork;User Id=xxx;Password=xxx;Encrypt=True;TrustServerCertificate=False;
JWT__Key=your-secure-256-bit-key-here
JWT__Issuer=https://your-app.up.railway.app
JWT__Audience=https://your-app.up.railway.app
Cloudinary__Api=cloudinary://434244532219775:jf0OWJdWHU9Nt1oqG1IjQVyiBlQ@dhnjbohwa
AllowedOrigins=https://your-frontend.vercel.app,http://localhost:3000
```

### B??c 4: Thêm Database (Optional - n?u c?n SQL Server)
- Railway có PostgreSQL mi?n phí
- Ho?c s? d?ng **Azure SQL Free Tier**

---

## ?? Option 2: Deploy lên Render (Free)

### B??c 1: Chu?n b?
Gi?ng Railway, push code lên GitHub

### B??c 2: Deploy
1. Truy c?p [render.com](https://render.com)
2. **New** ? **Web Service**
3. Connect GitHub repo
4. Ch?n **Docker** runtime
5. Thêm Environment Variables

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

## ?? Option 3: Deploy lên Azure (T?t nh?t cho .NET)

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

### Thêm Environment Variables trên Azure
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

## ?? Option 4: Deploy lên Fly.io (Free)

### B??c 1: Cài ??t Fly CLI
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

1. **KHÔNG commit file `.env` lên GitHub**
2. **??i JWT Key** - t?o key m?i cho production
3. **T?o Cloudinary account m?i** cho production
4. **S? d?ng HTTPS** cho t?t c? endpoints
5. **C?p nh?t AllowedOrigins** v?i domain frontend th?c t?

---

## ?? Checklist Tr??c Khi Deploy

- [ ] ?ã push code lên GitHub
- [ ] Dockerfile ? th? m?c root
- [ ] ?ã t?o database trên cloud (Azure SQL)
- [ ] ?ã có Cloudinary account
- [ ] ?ã generate JWT Key m?i cho production
- [ ] ?ã config AllowedOrigins v?i frontend URL
- [ ] ?ã test build local v?i Docker:
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
- Ki?m tra `AllowedOrigins` có ?úng URL frontend không
- ??m b?o không có trailing slash

### L?i JWT
- Ki?m tra JWT Key ?? dài (min 32 characters)
- Issuer/Audience ph?i match v?i domain API

### SignalR không ho?t ??ng
- M?t s? free tier không h? tr? WebSocket
- C?n upgrade plan ho?c dùng Long Polling fallback
