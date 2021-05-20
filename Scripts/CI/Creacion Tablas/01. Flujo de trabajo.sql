/**
Autor: Jonathan Edgarod Atencia Barranco
Fecha: 24/02/2021
Descripcion: Creacion de tablas para el flujo de trabajo
*/
CREATE TABLE Flujos 
(Id int identity(1,1) primary key not null, 
Accion int not null,
IdEmpresa int not null,
IdElemento int not null,
Elemento nvarchar(200) not null,
Tipo int not null,
)

CREATE TABLE Tareas 
(Id int identity(1,1) primary key not null, 
Proceso int not null,
IdPadre int null,
IdFlujo int null,
Orden int not null,
IdConfiguracion int null,
)