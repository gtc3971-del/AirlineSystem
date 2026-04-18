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

CREATE TABLE [Aircrafts] (
    [Id] int NOT NULL IDENTITY,
    [TailNumber] nvarchar(max) NOT NULL,
    [Model] nvarchar(max) NOT NULL,
    [TotalSeats] int NOT NULL,
    [TechnicalStatus] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Aircrafts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Airports] (
    [Code] nvarchar(450) NOT NULL,
    [Id] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [Country] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Airports] PRIMARY KEY ([Code])
);
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [PassportNumber] nvarchar(20) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [UserRole] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastLoginAt] datetime2 NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Bookings] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [BookingDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Passengers] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [PassportNumber] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Passengers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Flights] (
    [Id] int NOT NULL IDENTITY,
    [FlightNumber] nvarchar(450) NOT NULL,
    [DepartureTime] datetime2 NOT NULL,
    [ArrivalTime] datetime2 NOT NULL,
    [DepartureAirportCode] nvarchar(450) NOT NULL,
    [ArrivalAirportCode] nvarchar(450) NOT NULL,
    [AvailableSeats] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Flights] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Flights_Airports_ArrivalAirportCode] FOREIGN KEY ([ArrivalAirportCode]) REFERENCES [Airports] ([Code]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Flights_Airports_DepartureAirportCode] FOREIGN KEY ([DepartureAirportCode]) REFERENCES [Airports] ([Code]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Tickets] (
    [Id] int NOT NULL IDENTITY,
    [FlightId] int NOT NULL,
    [PassengerId] int NOT NULL,
    [SeatNumber] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [PurchaseDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tickets_Flights_FlightId] FOREIGN KEY ([FlightId]) REFERENCES [Flights] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tickets_Passengers_PassengerId] FOREIGN KEY ([PassengerId]) REFERENCES [Passengers] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'City', N'Country', N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Airports]'))
    SET IDENTITY_INSERT [Airports] ON;
INSERT INTO [Airports] ([Code], [City], [Country], [Id], [Name])
VALUES (N'KZN', N'Казань', N'Россия', 0, N'Кольцово'),
(N'LED', N'Санкт-Петербург', N'Россия', 0, N'Пулково'),
(N'SVO', N'Москва', N'Россия', 0, N'Шереметьево');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'City', N'Country', N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Airports]'))
    SET IDENTITY_INSERT [Airports] OFF;
GO

CREATE UNIQUE INDEX [IX_Airports_Code] ON [Airports] ([Code]);
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

CREATE INDEX [IX_Flights_ArrivalAirportCode] ON [Flights] ([ArrivalAirportCode]);
GO

CREATE INDEX [IX_Flights_DepartureAirportCode] ON [Flights] ([DepartureAirportCode]);
GO

CREATE UNIQUE INDEX [IX_Flights_FlightNumber] ON [Flights] ([FlightNumber]);
GO

CREATE INDEX [IX_Tickets_FlightId] ON [Tickets] ([FlightId]);
GO

CREATE INDEX [IX_Tickets_PassengerId] ON [Tickets] ([PassengerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260418120634_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

