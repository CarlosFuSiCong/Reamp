# Shared SQL Server & Reamp Backend Deployment

## 架构说明
- **共享 SQL Server**: 一个容器支持多个项目数据库
- **Reamp API**: 连接到共享 SQL Server

## VPS 部署步骤

### 1. 启动共享 SQL Server
```bash
# 进入 SQL Server 目录
cd /opt/reamp/docker/sqlserver

# 创建 .env 文件
cp .env.example .env
nano .env  # 设置强密码

# 启动 SQL Server（会创建 shared_sql_network）
docker-compose up -d

# 检查状态
docker-compose ps
docker-compose logs -f
```

### 2. 启动 Reamp API
```bash
# 进入 Reamp 后端目录
cd /opt/reamp/backend/docker

# 配置 .env
cp .env.example .env
nano .env
# 设置:
# - SQL_SERVER_HOST=shared-sqlserver
# - SQL_SERVER_PASSWORD=（与 SQL Server .env 相同）
# - DB_NAME=ReampDb
# - Cloudinary 配置
# - FRONTEND_URL=https://your-app.vercel.app

# 启动 API（连接到共享网络）
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# 检查连接
docker-compose logs -f
```

### 3. 添加第二个项目
```bash
# 项目2的 docker-compose.yml 也使用:
networks:
  shared-network:
    external: true
    name: shared_sql_network

# 连接字符串:
Server=shared-sqlserver,1433;Database=Project2Db;...
```

## 资源占用（2GB VPS）
- **SQL Server**: 900MB（支持多个数据库）
- **Reamp API**: 300MB
- **Project2 API**: ~300MB
- **总计**: ~1.5GB
- **剩余**: ~500MB 给系统

## 管理命令

### SQL Server
```bash
cd /opt/reamp/docker/sqlserver

# 查看日志
docker-compose logs -f

# 备份数据库
docker exec shared-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourPassword' \
  -Q "BACKUP DATABASE ReampDb TO DISK='/var/opt/mssql/backups/ReampDb.bak'"

# 停止（会影响所有项目）
docker-compose down
```

### Reamp API
```bash
cd /opt/reamp/backend/docker

# 重启
docker-compose -f docker-compose.yml -f docker-compose.prod.yml restart

# 查看日志
docker-compose logs -f api

# 停止（不影响 SQL Server）
docker-compose down
```

## 网络连接
所有项目通过 `shared_sql_network` 连接到同一个 SQL Server。
