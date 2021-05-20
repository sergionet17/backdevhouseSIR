CREATE TABLE [dbo].[Tareas] (
    [Id]      INT IDENTITY (1, 1) NOT NULL,
    [Proceso] INT NOT NULL,
    [IdPadre] INT NULL,
    [IdFlujo] INT NULL,
    [Orden]   INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tareas_Flujos] FOREIGN KEY ([IdFlujo]) REFERENCES [dbo].[Flujos] ([Id])
);



