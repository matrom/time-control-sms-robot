USE [master]
GO

/****** Object:  Database [TimeControlServer]    Script Date: 04/06/2012 17:08:03 ******/
CREATE DATABASE [TimeControlServer] ON  PRIMARY 
( NAME = N'TimeControlServer', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\TimeControlServer.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TimeControlServer_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\TimeControlServer_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [TimeControlServer] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TimeControlServer].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [TimeControlServer] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [TimeControlServer] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [TimeControlServer] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [TimeControlServer] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [TimeControlServer] SET ARITHABORT OFF 
GO

ALTER DATABASE [TimeControlServer] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [TimeControlServer] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [TimeControlServer] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [TimeControlServer] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [TimeControlServer] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [TimeControlServer] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [TimeControlServer] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [TimeControlServer] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [TimeControlServer] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [TimeControlServer] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [TimeControlServer] SET  DISABLE_BROKER 
GO

ALTER DATABASE [TimeControlServer] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [TimeControlServer] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [TimeControlServer] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [TimeControlServer] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [TimeControlServer] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [TimeControlServer] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [TimeControlServer] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [TimeControlServer] SET  READ_WRITE 
GO

ALTER DATABASE [TimeControlServer] SET RECOVERY FULL 
GO

ALTER DATABASE [TimeControlServer] SET  MULTI_USER 
GO

ALTER DATABASE [TimeControlServer] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [TimeControlServer] SET DB_CHAINING OFF 
GO

