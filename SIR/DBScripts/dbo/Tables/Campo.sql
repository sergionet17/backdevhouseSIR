CREATE TABLE [dbo].[Campo] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [IdEmpresa]     INT           NOT NULL,
    [IdServicio]    INT           NOT NULL,
    [IdReporte]     INT           NOT NULL,
    [Nombre]        VARCHAR (500) NULL,
    [Tipo]          INT           NOT NULL,
    [largo]         VARCHAR (10)  NULL,
    [Ordinal]       INT           NULL,
    [esVersionable] BIT           CONSTRAINT [DF_Campo_esVersionable] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Campo_Empresa] FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresa] ([IdEmpresa]),
    CONSTRAINT [FK_Campo_Reporte] FOREIGN KEY ([IdReporte]) REFERENCES [dbo].[Reporte] ([IdReporte]),
    CONSTRAINT [FK_Campo_Servicio] FOREIGN KEY ([IdServicio]) REFERENCES [dbo].[Servicio] ([IdServicio])
);



