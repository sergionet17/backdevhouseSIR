CREATE TABLE [dbo].[Reporte] (
    [IdReporte]   INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50)   NOT NULL,
    [Activo]      BIT            DEFAULT ((1)) NULL,
    [Norma]       NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([IdReporte] ASC)
);



