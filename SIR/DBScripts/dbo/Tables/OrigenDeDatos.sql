CREATE TABLE [dbo].[OrigenDeDatos] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [IdTarea]    INT           NOT NULL,
    [nombre]     VARCHAR (500) NOT NULL,
    [tipoOrigen] INT           NULL,
    [Usuario]    VARCHAR (200) NULL,
    [Clave]      VARCHAR (200) NULL,
    [Servidor]   VARCHAR (200) NULL,
    [SID]        VARCHAR (200) NULL,
    [Puerto]     VARCHAR (10)  NULL,
    [consulta]   VARCHAR (MAX) NULL,
    [tipoMando]  INT           NULL,
    CONSTRAINT [PK_OrigenDeDatos] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_OrigenDeDatos_Tareas] FOREIGN KEY ([IdTarea]) REFERENCES [dbo].[Tareas] ([Id])
);



