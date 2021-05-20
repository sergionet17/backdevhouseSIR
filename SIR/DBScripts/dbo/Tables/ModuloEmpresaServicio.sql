CREATE TABLE [dbo].[ModuloEmpresaServicio] (
    [IdModuloEmpresaServicio] INT IDENTITY (1, 1) NOT NULL,
    [IdEmpresaServicio]       INT NOT NULL,
    [IdModulo]                INT NOT NULL,
    [Activo]                  BIT DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([IdModuloEmpresaServicio] ASC),
    CONSTRAINT [FK_IdEmpresaServicio2] FOREIGN KEY ([IdEmpresaServicio]) REFERENCES [dbo].[EmpresaServicio] ([IdEmpresaServicio]),
    CONSTRAINT [FK_IdModulo] FOREIGN KEY ([IdModulo]) REFERENCES [dbo].[Modulo] ([IdModulo])
);

