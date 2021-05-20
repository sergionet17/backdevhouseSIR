CREATE TABLE [dbo].[ReporteEmpresaServicio] (
    [IdReporte]         INT NOT NULL,
    [IdEmpresaServicio] INT NOT NULL,
    [Activo]            BIT DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([IdReporte] ASC, [IdEmpresaServicio] ASC),
    CONSTRAINT [FK_IdEmpresaServicio] FOREIGN KEY ([IdEmpresaServicio]) REFERENCES [dbo].[EmpresaServicio] ([IdEmpresaServicio]),
    CONSTRAINT [FK_IdReporte] FOREIGN KEY ([IdReporte]) REFERENCES [dbo].[Reporte] ([IdReporte])
);

