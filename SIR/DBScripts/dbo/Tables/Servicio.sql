CREATE TABLE [dbo].[Servicio] (
    [IdServicio]  INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]      VARCHAR (50)  NULL,
    [Descripcion] VARCHAR (500) NOT NULL,
    CONSTRAINT [PK__Servicio__2DCCF9A254C069F9] PRIMARY KEY CLUSTERED ([IdServicio] ASC)
);



