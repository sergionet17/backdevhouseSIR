CREATE TABLE [dbo].[Condiciones] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [IdCampo]        INT           NOT NULL,
    [IdHomologacion] INT           NOT NULL,
    [Valor]          VARCHAR (500) NULL,
    [Condicion]      INT           NOT NULL,
    [Conector]       INT           NULL,
    [Nivel]          INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [FK_Condiciones_Campo] FOREIGN KEY ([IdCampo]) REFERENCES [dbo].[Campo] ([Id]),
    CONSTRAINT [FK_Condiciones_Homologaciones] FOREIGN KEY ([IdHomologacion]) REFERENCES [dbo].[Homologaciones] ([id])
);

