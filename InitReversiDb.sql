DROP DATABASE IF EXISTS ReversiDbContext
GO
CREATE DATABASE ReversiDbContext
GO
USE ReversiDbContext
GO

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

CREATE TABLE [Spel] (
    [ID] int NOT NULL IDENTITY,
    [Omschrijving] nvarchar(max) NULL,
    [Token] nvarchar(max) NULL,
    [Speler1Token] nvarchar(max) NULL,
    [Speler2Token] nvarchar(max) NULL,
    [Bord] nvarchar(max) NULL,
    [AandeBeurt] int NOT NULL,
    CONSTRAINT [PK_Spel] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Spelers] (
    [Id] uniqueidentifier NOT NULL,
    [Naam] nvarchar(max) NULL,
    [AantalGewonnen] int NOT NULL,
    [AantalVerloren] int NOT NULL,
    [AantalGelijk] int NOT NULL,
    CONSTRAINT [PK_Spelers] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230928083112_InitialCreate', N'5.0.17');
GO

COMMIT;
GO
