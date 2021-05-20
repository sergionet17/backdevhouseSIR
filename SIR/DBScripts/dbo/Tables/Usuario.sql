CREATE TABLE [dbo].[Usuario] (
    [IdUsuario]         INT          IDENTITY (1, 1) NOT NULL,
    [Usuario]           VARCHAR (30) NOT NULL,
    [Activo]            BIT          DEFAULT ((1)) NULL,
    [FechaRegistro]     DATETIME     DEFAULT (getdate()) NOT NULL,
    [FechaUltimoLogin]  DATETIME     NULL,
    [FechaModificacion] DATETIME     NULL,
    [EsAdministrador]   BIT          NULL,
    [Nombre]            VARCHAR (50) NULL,
    [Apellido]          VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([IdUsuario] ASC),
    UNIQUE NONCLUSTERED ([Usuario] ASC)
);

