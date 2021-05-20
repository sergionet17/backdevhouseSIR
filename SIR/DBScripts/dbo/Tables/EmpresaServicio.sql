CREATE TABLE [dbo].[EmpresaServicio] (
    [IdEmpresaServicio] INT IDENTITY (1, 1) NOT NULL,
    [IdEmpresa]         INT NOT NULL,
    [IdServicio]        INT NOT NULL,
    [Activo]            BIT DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([IdEmpresaServicio] ASC),
    CONSTRAINT [FK_IdEmpresa] FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresa] ([IdEmpresa]),
    CONSTRAINT [FK_IdServicio] FOREIGN KEY ([IdServicio]) REFERENCES [dbo].[Servicio] ([IdServicio])
);

