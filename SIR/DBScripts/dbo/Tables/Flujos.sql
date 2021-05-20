CREATE TABLE [dbo].[Flujos] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Accion]     INT            NOT NULL,
    [IdEmpresa]  INT            NOT NULL,
    [IdElemento] INT            NOT NULL,
    [Elemento]   NVARCHAR (200) NOT NULL,
    [Tipo]       INT            NOT NULL,
    [IdServicio] INT            CONSTRAINT [DF_Flujos_IdServicio] DEFAULT ((0)) NOT NULL,
    [SubTipo]    INT            CONSTRAINT [DF_Flujos_SubTipo] DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);





