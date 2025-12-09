IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] uniqueidentifier NOT NULL,
    [Description] nvarchar(max) NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] uniqueidentifier NOT NULL,
    [DisplayName] nvarchar(max) NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] uniqueidentifier NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250814110935_InitIdentity', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'AvatarUrl');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [AvatarUrl];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'DisplayName');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [DisplayName];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetRoles]') AND [c].[name] = N'Description');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetRoles] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [AspNetRoles] DROP COLUMN [Description];
GO

ALTER TABLE [AspNetUsers] ADD [DeletedAt] datetime2 NULL;
GO

ALTER TABLE [AspNetUsers] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250815030437_Update_Identity_UserRole', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'IsDeleted');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [IsDeleted];
GO

EXEC sp_rename N'[AspNetUsers].[DeletedAt]', N'DeletedAtUtc', N'COLUMN';
GO

EXEC sp_rename N'[AspNetUsers].[CreatedAt]', N'CreatedAtUtc', N'COLUMN';
GO

CREATE TABLE [UserProfiles] (
    [Id] uniqueidentifier NOT NULL,
    [ApplicationUserId] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(40) NOT NULL,
    [LastName] nvarchar(40) NOT NULL,
    [AvatarUrl] nvarchar(256) NULL,
    [Role] int NOT NULL DEFAULT 0,
    [Status] int NOT NULL DEFAULT 0,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserProfiles_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_UserProfiles_ApplicationUserId] ON [UserProfiles] ([ApplicationUserId]) WHERE [DeletedAtUtc] IS NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250821085333_Init_Accounts', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserProfiles]') AND [c].[name] = N'Role');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [UserProfiles] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [UserProfiles] ADD DEFAULT 1 FOR [Role];
GO

CREATE TABLE [Agencies] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Slug] nvarchar(140) NOT NULL,
    [Description] nvarchar(512) NULL,
    [LogoUrl] nvarchar(256) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [ContactEmail] nvarchar(120) NOT NULL,
    [ContactPhone] nvarchar(40) NOT NULL,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_Agencies] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Agencies_ContactEmail_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactEmail]))) > 0),
    CONSTRAINT [CK_Agencies_ContactPhone_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactPhone]))) > 0)
);
GO

CREATE TABLE [Studios] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Slug] nvarchar(140) NOT NULL,
    [Description] nvarchar(512) NULL,
    [LogoUrl] nvarchar(256) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [ContactEmail] nvarchar(120) NOT NULL,
    [ContactPhone] nvarchar(40) NOT NULL,
    [Address_Line1] nvarchar(120) NULL,
    [Address_Line2] nvarchar(120) NULL,
    [Address_City] nvarchar(80) NULL,
    [Address_State] nvarchar(40) NULL,
    [Address_Postcode] nvarchar(10) NULL,
    [Address_Country] nvarchar(2) NULL,
    [Address_Latitude] float(9) NULL,
    [Address_Longitude] float(9) NULL,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_Studios] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Studios_Address_Lat] CHECK ([Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)),
    CONSTRAINT [CK_Studios_Address_Lng] CHECK ([Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)),
    CONSTRAINT [CK_Studios_ContactEmail_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactEmail]))) > 0),
    CONSTRAINT [CK_Studios_ContactPhone_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactPhone]))) > 0)
);
GO

CREATE TABLE [AgencyBranches] (
    [Id] uniqueidentifier NOT NULL,
    [AgencyId] uniqueidentifier NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Slug] nvarchar(140) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [ContactEmail] nvarchar(120) NOT NULL,
    [ContactPhone] nvarchar(40) NOT NULL,
    [Address_Line1] nvarchar(120) NULL,
    [Address_Line2] nvarchar(120) NULL,
    [Address_City] nvarchar(80) NULL,
    [Address_State] nvarchar(40) NULL,
    [Address_Postcode] nvarchar(10) NULL,
    [Address_Country] nvarchar(2) NULL,
    [Address_Latitude] float(9) NULL,
    [Address_Longitude] float(9) NULL,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_AgencyBranches] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_AgencyBranches_Address_Lat] CHECK ([Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)),
    CONSTRAINT [CK_AgencyBranches_Address_Lng] CHECK ([Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)),
    CONSTRAINT [CK_AgencyBranches_ContactEmail_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactEmail]))) > 0),
    CONSTRAINT [CK_AgencyBranches_ContactPhone_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ContactPhone]))) > 0),
    CONSTRAINT [FK_AgencyBranches_Agencies_AgencyId] FOREIGN KEY ([AgencyId]) REFERENCES [Agencies] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Staffs] (
    [Id] uniqueidentifier NOT NULL,
    [UserProfileId] uniqueidentifier NOT NULL,
    [StudioId] uniqueidentifier NOT NULL,
    [Skills] int NOT NULL,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_Staffs] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Staffs_Skills_Valid] CHECK ([Skills] >= 0),
    CONSTRAINT [FK_Staffs_Studios_StudioId] FOREIGN KEY ([StudioId]) REFERENCES [Studios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Staffs_UserProfiles_UserProfileId] FOREIGN KEY ([UserProfileId]) REFERENCES [UserProfiles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Clients] (
    [Id] uniqueidentifier NOT NULL,
    [UserProfileId] uniqueidentifier NOT NULL,
    [AgencyId] uniqueidentifier NOT NULL,
    [AgencyBranchId] uniqueidentifier NULL,
    [RowVersion] rowversion NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Clients_Agencies_AgencyId] FOREIGN KEY ([AgencyId]) REFERENCES [Agencies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Clients_AgencyBranches_AgencyBranchId] FOREIGN KEY ([AgencyBranchId]) REFERENCES [AgencyBranches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Clients_UserProfiles_UserProfileId] FOREIGN KEY ([UserProfileId]) REFERENCES [UserProfiles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Agencies_CreatedBy] ON [Agencies] ([CreatedBy]);
GO

CREATE UNIQUE INDEX [IX_Agencies_Slug] ON [Agencies] ([Slug]) WHERE [DeletedAtUtc] IS NULL;
GO

CREATE INDEX [IX_AgencyBranches_Address_City] ON [AgencyBranches] ([Address_City]);
GO

CREATE UNIQUE INDEX [IX_AgencyBranches_AgencyId_Slug] ON [AgencyBranches] ([AgencyId], [Slug]) WHERE [DeletedAtUtc] IS NULL;
GO

CREATE INDEX [IX_AgencyBranches_CreatedBy] ON [AgencyBranches] ([CreatedBy]);
GO

CREATE INDEX [IX_Clients_AgencyBranchId] ON [Clients] ([AgencyBranchId]);
GO

CREATE INDEX [IX_Clients_AgencyId] ON [Clients] ([AgencyId]);
GO

CREATE UNIQUE INDEX [IX_Clients_UserProfileId] ON [Clients] ([UserProfileId]);
GO

CREATE INDEX [IX_Staffs_StudioId] ON [Staffs] ([StudioId]);
GO

CREATE UNIQUE INDEX [IX_Staffs_UserProfileId] ON [Staffs] ([UserProfileId]);
GO

CREATE INDEX [IX_Studios_Address_City] ON [Studios] ([Address_City]);
GO

CREATE INDEX [IX_Studios_CreatedBy] ON [Studios] ([CreatedBy]);
GO

CREATE UNIQUE INDEX [IX_Studios_Slug] ON [Studios] ([Slug]) WHERE [DeletedAtUtc] IS NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250823124316_Refactor_Slug_ValueObject_All', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserProfiles]') AND [c].[name] = N'Status');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [UserProfiles] DROP CONSTRAINT [' + @var5 + '];');
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserProfiles]') AND [c].[name] = N'Role');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [UserProfiles] DROP CONSTRAINT [' + @var6 + '];');
GO

ALTER TABLE [UserProfiles] ADD CONSTRAINT [CK_UserProfiles_FirstName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([FirstName]))) > 0);
GO

ALTER TABLE [UserProfiles] ADD CONSTRAINT [CK_UserProfiles_LastName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([LastName]))) > 0);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250824095449_Init_Accounts_v3', N'8.0.19');
GO

COMMIT;
GO

