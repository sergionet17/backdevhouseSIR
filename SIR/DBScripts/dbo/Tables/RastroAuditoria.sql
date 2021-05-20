CREATE TABLE [dbo].[RastroAuditoria] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [TipoAccionAuditoria] INT            NULL,
    [ModeloDatos]         VARCHAR (500)  NULL,
    [FechaAccion]         DATETIME       NULL,
    [CampoClaveID]        BIGINT         NULL,
    [ValorAntes]          NVARCHAR (MAX) NULL,
    [ValorDespues]        NVARCHAR (MAX) NULL,
    [Cambios]             NVARCHAR (MAX) NULL,
    [Usuario]             VARCHAR (30)   NULL,
    [Ip]                  VARCHAR (20)   NULL,
    CONSTRAINT [PK_RastroAuditoria] PRIMARY KEY CLUSTERED ([Id] ASC)
);

