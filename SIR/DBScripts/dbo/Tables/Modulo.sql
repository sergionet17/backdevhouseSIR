CREATE TABLE [dbo].[Modulo] (
    [IdModulo]    INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    [Activo]      BIT          DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([IdModulo] ASC)
);

