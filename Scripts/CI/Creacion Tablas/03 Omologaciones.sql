USE [SIR2]
GO

CREATE TABLE [dbo].[Omologaciones](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[IdTarea] [int] NOT NULL,
	[IdCampo] [int] NOT NULL,
	[ValorOriginal] [varchar](500) NULL,
	[ValorNuevo] [varchar](500) NULL
) 
GO


