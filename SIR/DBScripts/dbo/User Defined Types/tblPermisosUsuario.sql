CREATE TYPE [dbo].[tblPermisosUsuario] AS TABLE (
    [IdUsuario]               INT NOT NULL,
    [IdModuloEmpresaServicio] INT NOT NULL,
    [TipoPermiso]             INT NOT NULL,
    PRIMARY KEY CLUSTERED ([IdUsuario] ASC, [IdModuloEmpresaServicio] ASC));



