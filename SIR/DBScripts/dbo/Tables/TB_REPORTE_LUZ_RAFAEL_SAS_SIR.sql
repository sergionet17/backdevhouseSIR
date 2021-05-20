CREATE TABLE [dbo].[TB_REPORTE_LUZ_RAFAEL_SAS_SIR] (
    [ID_TABLA]   INT            IDENTITY (1, 1) NOT NULL,
    [Version]    INT            NOT NULL,
    [IdFlujo]    INT            NOT NULL,
    [ID]         INT            NULL,
    [GESTOR]     NVARCHAR (200) NULL,
    [NOTIQUETES] NVARCHAR (20)  NULL,
    [VENCIDOS]   INT            NULL,
    [FECHA]      DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID_TABLA] ASC)
);

