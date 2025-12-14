# 角色系统重构 - Docker 和数据库更新指南

## 更新日期：2024-12-14

## 一、角色系统变更摘要

### 1. UserRole 枚举更新
```csharp
// 旧版本
None = 0,
User = 1,
Client = 2,    // 已删除（改为 Agent）
Staff = 3,
Admin = 4

// 新版本
None = 0,
User = 1,      // 所有注册用户的默认角色（保持不变）
Agent = 2,     // Agency 成员的用户级别角色（原 Client）
Staff = 3,
Admin = 4
```

### 2. AgencyRole 枚举更新
```csharp
// 旧版本
Member = 0,    // 已移除
Agent = 1,
Manager = 2,
Owner = 3

// 新版本
Agent = 1,     // 最低角色
Manager = 2,
Owner = 3
```

### 3. StudioRole 枚举更新
```csharp
// 旧版本
Member = 0,        // 已移除
Editor = 1,        // 已移除
Photographer = 2,  // 已移除
Manager = 3,
Owner = 4

// 新版本
Staff = 1,     // 统一的员工角色（替代 Member/Editor/Photographer）
Manager = 2,   // 从 3 改为 2
Owner = 3      // 从 4 改为 3
```

## 二、数据库迁移

### 迁移文件
- **文件名**: `20251214013949_UpdateRoleEnums.cs`
- **位置**: `backend/src/Reamp/Reamp.Infrastructure/Migrations/`

### 迁移内容

#### UserProfiles 表
```sql
-- User(1) 保持为 1，但语义变为 Client
-- Client(2) 变为 Agent(2)
UPDATE UserProfiles SET Role = 10 WHERE Role = 2;  -- 临时值
UPDATE UserProfiles SET Role = 2 WHERE Role = 10;   -- Client -> Agent
```

#### Agents 表
```sql
-- Member(0) 变为 Agent(1)
UPDATE Agents SET Role = 1 WHERE Role = 0;
```

#### Staff 表
```sql
UPDATE Staff 
SET Role = CASE 
    WHEN Role = 0 THEN 1  -- Member -> Staff
    WHEN Role = 1 THEN 1  -- Editor -> Staff
    WHEN Role = 2 THEN 1  -- Photographer -> Staff
    WHEN Role = 3 THEN 2  -- Manager -> Manager
    WHEN Role = 4 THEN 3  -- Owner -> Owner
    ELSE Role
END;
```

## 三、Docker 配置

### 文件结构
```
backend/docker/
├── docker-compose.yml           # 主配置文件
├── docker-compose.override.yml  # 开发环境覆盖
├── docker-compose.prod.yml      # 生产环境配置
├── Dockerfile                   # API 镜像构建
├── .env.example                 # 环境变量模板
├── README.md                    # Docker 使用文档
├── rebuild.ps1                  # Windows 重建脚本
├── rebuild.sh                   # Linux/Mac 重建脚本
├── reset-database.ps1           # Windows 数据库重置
├── reset-database.sh            # Linux/Mac 数据库重置
└── check-database.ps1           # 数据库检查脚本
```

### 环境变量配置

1. 复制环境变量模板：
```bash
cd backend/docker
cp .env.example .env
```

2. 编辑 `.env` 文件，设置实际值：
```env
SQL_SERVER_PASSWORD=YourActualPassword
JWT_SECRET=YourActual32CharacterSecretKey
CLOUDINARY_CLOUD_NAME=your_actual_cloud
# ... 其他配置
```

## 四、部署步骤

### 方案 A：全新部署（推荐用于测试环境）

1. **停止现有服务**
```powershell
cd backend/docker
docker-compose down
```

2. **清除旧数据**
```powershell
docker volume rm reamp_sqlserver_data
```

3. **启动新服务**
```powershell
docker-compose up -d
```

4. **验证迁移**
```powershell
docker-compose logs -f api
# 查看迁移日志，确认 UpdateRoleEnums 已应用
```

### 方案 B：保留数据升级（推荐用于生产环境）

1. **备份当前数据库**
```powershell
# 连接到 SQL Server 容器
docker exec -it reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword" -C

# 执行备份
BACKUP DATABASE ReampDb TO DISK = '/var/opt/mssql/backup/ReampDb.bak'
GO
```

2. **更新代码**
```bash
git pull origin feat/invitation-member-system
```

3. **重建 Docker 镜像**
```powershell
.\rebuild.ps1
```

4. **验证角色数据**
```powershell
.\check-database.ps1
```

## 五、验证检查清单

### 1. 数据库迁移验证
```sql
-- 检查迁移历史
SELECT * FROM __EFMigrationsHistory 
WHERE MigrationId LIKE '%UpdateRoleEnums%';

-- 检查 UserProfile 角色分布
SELECT Role, COUNT(*) as Count FROM UserProfiles GROUP BY Role;
-- 预期：没有 Role = 1 (旧 User)，Client = 1, Agent = 2

-- 检查 Agent 角色分布
SELECT Role, COUNT(*) as Count FROM Agents WHERE DeletedAtUtc IS NULL GROUP BY Role;
-- 预期：没有 Role = 0，最小值为 1 (Agent)

-- 检查 Staff 角色分布
SELECT Role, COUNT(*) as Count FROM Staff WHERE DeletedAtUtc IS NULL GROUP BY Role;
-- 预期：没有 0,1,2；只有 1(Staff), 2(Manager), 3(Owner)
```

### 2. API 功能验证
- [ ] 用户注册（默认角色应为 Client）
- [ ] Agency 申请提交
- [ ] Studio 申请提交
- [ ] Admin 审核申请
- [ ] Owner 邀请成员
- [ ] Agency 角色管理（Agent, Manager, Owner）
- [ ] Studio 角色管理（Staff, Manager, Owner）

### 3. 前端显示验证
- [ ] 角色 Badge 正确显示
- [ ] 邀请对话框只显示新角色
- [ ] 团队管理页面角色下拉正确
- [ ] 首页根据 Client 角色显示正确选项

## 六、回滚方案

如果需要回滚到旧版本：

```powershell
# 1. 停止服务
docker-compose down

# 2. 恢复代码
git checkout main

# 3. 恢复数据库备份
docker exec -it reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword" -C
RESTORE DATABASE ReampDb FROM DISK = '/var/opt/mssql/backup/ReampDb.bak' WITH REPLACE
GO

# 4. 重启服务
docker-compose up -d
```

## 七、常见问题

### Q1: 迁移失败怎么办？
**A**: 检查日志，确认是否有数据约束冲突：
```powershell
docker-compose logs api | Select-String -Pattern "migration|error"
```

### Q2: 如何查看当前数据库角色分布？
**A**: 使用 `check-database.ps1` 脚本：
```powershell
.\check-database.ps1
```

### Q3: Docker 镜像构建失败？
**A**: 清除缓存重新构建：
```powershell
docker-compose build --no-cache
```

### Q4: 数据库连接失败？
**A**: 检查密码和连接字符串：
```powershell
# 查看环境变量
docker-compose config

# 测试数据库连接
docker exec -it reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword" -C
```

## 八、技术支持

如遇问题，请检查：
1. Docker 日志：`docker-compose logs -f`
2. API 日志：`backend/logs/` 目录
3. 数据库状态：`check-database.ps1`

## 九、相关文档

- [Docker README](./README.md) - Docker 详细使用说明
- [Common Rules](../../.cursor/rules/common.mdc) - 代码提交规范
- [Backend C# Rules](../../.cursor/rules/backendc.mdc) - 后端开发规范
- [Next.js Rules](../../.cursor/rules/nextjs.mdc) - 前端开发规范
