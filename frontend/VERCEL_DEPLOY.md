# Vercel 部署配置说明

## 1. 在 Vercel 创建项目
- 访问 https://vercel.com
- Import Git Repository
- 选择你的 GitHub 仓库
- Root Directory: `frontend`
- Framework Preset: Next.js

## 2. 环境变量配置
在 Vercel 项目设置中添加：

```bash
# 必需
NEXT_PUBLIC_API_URL=https://your-vps-domain.com  # VPS 后端 API 地址
NEXT_PUBLIC_APP_URL=https://your-app.vercel.app

# 可选
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_key
NEXT_PUBLIC_ENABLE_DEBUG=false
```

## 3. VPS 后端 CORS 配置
确保后端 .env 包含 Vercel 域名：

```bash
FRONTEND_URL=https://your-app.vercel.app
FRONTEND_URL_2=https://your-app.vercel.app
```

## 4. 部署
- 推送代码到 GitHub master 分支
- Vercel 自动部署
- 首次部署后获得 .vercel.app 域名
- 更新 VPS 后端的 CORS 配置
