CREATE TABLE [dbo].[PermisosUsuario] (
    [IdUsuario]               INT NOT NULL,
    [IdModuloEmpresaServicio] INT NOT NULL,
    [IdTipoPermiso]           INT NOT NULL,
    PRIMARY KEY CLUSTERED ([IdUsuario] ASC, [IdModuloEmpresaServicio] ASC, [IdTipoPermiso] ASC),
    CONSTRAINT [FK_IdModuloEmpresaUsuario] FOREIGN KEY ([IdModuloEmpresaServicio]) REFERENCES [dbo].[ModuloEmpresaServicio] ([IdModuloEmpresaServicio]),
    CONSTRAINT [FK_IdTipoPermiso] FOREIGN KEY ([IdTipoPermiso]) REFERENCES [dbo].[TipoPermiso] ([IdTipoPermiso]),
    CONSTRAINT [FK_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([IdUsuario])
);

