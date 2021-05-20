CREATE TABLE [dbo].[Empresa] (
    [IdEmpresa]            INT            IDENTITY (1, 1) NOT NULL,
    [NumeroIdentificacion] VARCHAR (20)   NOT NULL,
    [RazonSocial]          VARCHAR (100)  NULL,
    [Correo]               VARCHAR (230)  NULL,
    [Activo]               BIT            DEFAULT ((1)) NULL,
    [FechaCreacion]        DATETIME       DEFAULT (getdate()) NULL,
    [Descripcion]          VARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([IdEmpresa] ASC)
);

