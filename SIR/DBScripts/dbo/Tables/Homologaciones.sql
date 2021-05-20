CREATE TABLE [dbo].[Homologaciones] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [IdTarea]       INT           NOT NULL,
    [IdCampo]       INT           NOT NULL,
    [ValorNo]       VARCHAR (500) NULL,
    [ValorSi]       VARCHAR (500) NULL,
    [TipoReemplazo] INT           CONSTRAINT [DF_Homologaciones_TipoReemplazo] DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Omologaciones_Tareas] FOREIGN KEY ([IdTarea]) REFERENCES [dbo].[Tareas] ([Id])
);



