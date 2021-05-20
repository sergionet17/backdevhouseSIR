USE [SIR2]
GO

DROP TABLE [dbo].[OrigenDeDatos]
GO

CREATE TABLE [dbo].[OrigenDeDatos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_configuracion] [int] NULL,
	[nombre] [varchar](500) NOT NULL,
	[tipoOrigen] [int] NULL,
	[Usuario] [varchar](200) NULL,
	[Clave] [varchar](200) NULL,
	[Servidor] [varchar](200) NULL,
	[SID] [varchar](200) NULL,
	[Puerto] [varchar](10) NULL,
	[consulta] [varchar](MAX) NULL,
	[tipoMando] [int] NULL,
 CONSTRAINT [PK_OrigenDeDatos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


