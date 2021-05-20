CREATE TABLE [dbo].[TipoPermiso] (
    [IdTipoPermiso] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]   VARCHAR (50) NOT NULL,
    [Activo]        BIT          DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([IdTipoPermiso] ASC)
);

