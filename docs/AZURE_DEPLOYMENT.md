# Azure 部署指南

## 概述

本指南说明如何将代码更改部署到 Azure。

## 部署架构

- **前端**: Azure Static Web Apps (自动部署)
- **后端**: Azure Container Apps (手动/脚本部署)
- **数据库**: Azure SQL Database

## 部署步骤

### 1. 前端自动部署

前端通过 GitHub Actions 自动部署到 Azure Static Web Apps。

```bash
# 推送代码到 master 分支会自动触发部署
git push origin master
```

查看部署状态:
- 访问: https://github.com/CarlosFuSiCong/Reamp/actions
- 找到最新的 "Azure Static Web Apps CI/CD" workflow
- 等待部署完成 (通常需要 3-5 分钟)

### 2. 后端手动部署

#### 选项 A: 使用部署脚本 (推荐)

```powershell
cd backend/scripts

# 完整部署 (构建 + 更新)
.\deploy-to-azure.ps1 `
    -ResourceGroup "reamp-rg" `
    -AcrName "reampacr" `
    -ContainerAppName "reamp-api"

# 仅更新 (跳过构建)
.\deploy-to-azure.ps1 -SkipBuild

# 包含数据库迁移
.\deploy-to-azure.ps1 `
    -RunMigrations `
    -SqlServerName "reamp-sql-server"
```

#### 选项 B: 手动使用 Azure CLI

```bash
# 1. 登录 Azure
az login

# 2. 构建并推送到 ACR
cd backend
az acr build \
  --registry reampacr \
  --image reamp-api:latest \
  --file docker/Dockerfile \
  .

# 3. 更新 Container App
az containerapp update \
  --name reamp-api \
  --resource-group reamp-rg \
  --image reampacr.azurecr.io/reamp-api:latest
```

### 3. 数据库迁移 (如需要)

如果有新的数据库迁移:

```powershell
cd backend/scripts

# 运行迁移
.\deploy-database.ps1 -ConnectionString "Server=tcp:..."
```

或在部署脚本中包含 `-RunMigrations` 参数。

## 验证部署

### 前端验证

1. 访问前端 URL (检查 GitHub Actions 输出获取 URL)
2. 打开浏览器开发者工具 - Console
3. 上传测试头像
4. 检查是否有错误日志

### 后端验证

```bash
# 1. 检查健康状态
curl https://<container-app-url>/health

# 2. 查看实时日志
az containerapp logs show \
  --name reamp-api \
  --resource-group reamp-rg \
  --follow

# 3. 检查特定日志 (头像上传)
az containerapp logs show \
  --name reamp-api \
  --resource-group reamp-rg \
  --follow | grep -i "avatar"
```

### 验证修复

1. 登录应用
2. 进入 Profile 页面
3. 上传新头像
4. 检查浏览器 Console:
   - 应该看到 `[AvatarUpload] Upload successful:` 日志
   - 应该显示 assetId, publicUrl 等信息
5. 刷新页面，头像应该正确显示
6. 检查 URL 是否为干净的格式 (不包含重复的转换参数)

## 本次修复的变更

### 后端变更
- ✅ `CloudinaryService.cs` - 存储干净的 base URL (不含转换参数)
- ✅ `MediaAssetAppService.cs` - 添加详细日志
- ✅ `MediaController.cs` - 添加上传成功日志

### 前端变更
- ✅ `avatar-upload.tsx` - 添加错误处理和日志
- ✅ `cloudinary.ts` - 添加开发模式 debug 日志

## 环境变量检查

确保 Azure 上配置了以下环境变量:

### Container App 环境变量
```bash
# Cloudinary (必须)
CLOUDINARY_CLOUD_NAME=dccearggm
CLOUDINARY_API_KEY=<your-key>
CLOUDINARY_API_SECRET=<your-secret>

# Cloudinary Settings (生产环境)
CloudinarySettings__Folder=reamp  # 生产环境用 'reamp'，开发用 'reamp-dev'

# CORS
Cors__AllowedOrigins__0=https://<frontend-url>

# 数据库
ConnectionStrings__SqlServerConnection=<connection-string>

# JWT
JwtSettings__Secret=<secret>
JwtSettings__Issuer=<issuer>
JwtSettings__Audience=<audience>
```

### Static Web App 环境变量
```bash
NEXT_PUBLIC_API_URL=https://<container-app-url>
```

## 故障排查

### 问题: 头像仍然 404

1. **检查日志** - 查看后端日志，确认图片上传成功:
   ```bash
   az containerapp logs show --name reamp-api --resource-group reamp-rg --follow
   ```
   
   应该看到:
   ```
   Image uploaded to Cloudinary: PublicId=reamp/xyz, Format=jpg, BaseUrl=https://...
   Avatar uploaded successfully: AssetId=..., PublicUrl=..., ProviderAssetId=...
   ```

2. **检查 Cloudinary** - 登录 Cloudinary Console:
   - 访问: https://cloudinary.com/console/media_library
   - 搜索 public_id
   - 确认图片存在

3. **检查数据库** - 查询 MediaAssets 表:
   ```sql
   SELECT TOP 10 
       Id, 
       ProviderAssetId, 
       PublicUrl, 
       ProcessStatus,
       CreatedAtUtc
   FROM MediaAssets
   WHERE OwnerStudioId = '00000000-0000-0000-0000-000000000000'  -- 用户头像
   ORDER BY CreatedAtUtc DESC
   ```

4. **检查 URL 格式** - PublicUrl 应该是:
   ```
   https://res.cloudinary.com/dccearggm/image/upload/reamp/xyz.jpg
   ```
   
   而不是:
   ```
   https://res.cloudinary.com/dccearggm/image/upload/q_auto,f_auto/reamp/xyz.jpg
   ```

### 问题: Container App 更新失败

```bash
# 检查 Container App 状态
az containerapp show \
  --name reamp-api \
  --resource-group reamp-rg \
  --query "{provisioningState: properties.provisioningState, runningState: properties.runningStatus}"

# 检查 revisions
az containerapp revision list \
  --name reamp-api \
  --resource-group reamp-rg \
  --query "[].{name: name, active: properties.active, created: properties.createdTime}"
```

### 问题: ACR 推送失败

```bash
# 检查 ACR 登录状态
az acr login --name reampacr

# 检查 ACR 是否存在
az acr show --name reampacr --resource-group reamp-rg
```

## 快速命令参考

```bash
# 查看所有资源
az resource list --resource-group reamp-rg --output table

# 重启 Container App
az containerapp revision restart \
  --name reamp-api \
  --resource-group reamp-rg \
  --revision <revision-name>

# 查看环境变量
az containerapp show \
  --name reamp-api \
  --resource-group reamp-rg \
  --query "properties.template.containers[0].env"

# 更新环境变量
az containerapp update \
  --name reamp-api \
  --resource-group reamp-rg \
  --set-env-vars "NEW_VAR=value"
```

## 回滚

如果部署出现问题，可以回滚到之前的版本:

```bash
# 列出所有 revisions
az containerapp revision list \
  --name reamp-api \
  --resource-group reamp-rg \
  --output table

# 激活旧的 revision
az containerapp revision activate \
  --name reamp-api \
  --resource-group reamp-rg \
  --revision <previous-revision-name>
```

## 监控

### 应用洞察 (Application Insights)

如果配置了 Application Insights:

```bash
# 查询日志
az monitor app-insights query \
  --app <app-insights-name> \
  --analytics-query "traces | where message contains 'avatar' | order by timestamp desc | take 50"
```

### 度量指标

```bash
# CPU 使用率
az monitor metrics list \
  --resource <container-app-resource-id> \
  --metric "CpuPercentage" \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z
```

## 相关文档

- [Azure Container Apps 文档](https://docs.microsoft.com/azure/container-apps/)
- [Azure Static Web Apps 文档](https://docs.microsoft.com/azure/static-web-apps/)
- [Cloudinary 文档](https://cloudinary.com/documentation)
