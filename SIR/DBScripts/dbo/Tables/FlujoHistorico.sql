CREATE TABLE [dbo].[FlujoHistorico] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [IdEmpresa]         INT            NOT NULL,
    [IdServicio]        INT            NOT NULL,
    [IdElemento]        INT            NOT NULL,
    [TipoFlujo]         INT            NOT NULL,
    [IdTarea]           INT            NOT NULL,
    [FechaCreacion]     DATETIME       NULL,
    [FechaFinalizacion] DATETIME       NULL,
    [EsValido]          BIT            CONSTRAINT [DF_FlujoHistorico_EsValido] DEFAULT ((1)) NOT NULL,
    [DescripcionError]  NVARCHAR (500) NULL,
    [IdFlujo]           INT            NULL,
    CONSTRAINT [PK_FlujoHistorico] PRIMARY KEY CLUSTERED ([Id] ASC)
);

