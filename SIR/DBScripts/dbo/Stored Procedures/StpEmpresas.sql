-- =============================================
-- Author:		Carlos Torres
-- Create date: 12-02-2020
-- Description:	CRUD de empresas
-- =============================================
CREATE PROCEDURE [dbo].[StpEmpresas]
	-- Add the parameters for the stored procedure here
	@accion int = 0, 
	@IdEmpresa	int= 0, 
	@NumeroIdentificacion varchar(20) ='',
	@RazonSocial varchar(100) = '',
	@Correo	varchar(230) = '',
	@Activo	bit = 1,
	@FechaCreacion datetime = null,
	@servicios varchar(3000) = '',
	@Descripcion varchar(3000) = ''
AS
BEGIN
	DECLARE @ids table(id INT);

	if @accion = 1
	begin 
		select t0.*,t2.*
		from Empresa t0
		LEFT JOIN EmpresaServicio t1 ON t0.IdEmpresa = t1.IdEmpresa AND t1.Activo = 1
		LEFT JOIN Servicio t2 ON t1.IdServicio = t2.IdServicio
	end
	else if @accion = 2
	begin
		select *
		from Empresa
		where IdEmpresa = @IdEmpresa
	end
	else if @accion = 3
	begin
		BEGIN TRY
				insert into Empresa(NumeroIdentificacion,RazonSocial,Correo,Activo,FechaCreacion,Descripcion)
				OUTPUT inserted.IdEmpresa INTO @ids
				values(@NumeroIdentificacion,@RazonSocial,@Correo,@Activo,@FechaCreacion,@Descripcion);
				
				SELECT *
				FROM @ids;
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE();
		END CATCH
	end
	else if @accion = 4
	begin
		BEGIN TRY
				UPDATE Empresa SET NumeroIdentificacion = @NumeroIdentificacion,RazonSocial = @RazonSocial,Correo = @Correo,Activo = @Activo,Descripcion = @Descripcion
				WHERE IdEmpresa = @IdEmpresa
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE();
		END CATCH
	end
	else if @accion = 5
	begin
		BEGIN TRY
			delete from EmpresaServicio where IdEmpresa = @IdEmpresa
			delete from Empresa where IdEmpresa = @IdEmpresa		
		END TRY
		BEGIN CATCH
			SELECT N'No se puede borrar esta empresa ya que tiene relaciones con otros elementos';
		END CATCH
	end
	else if @accion = 6
	begin
		BEGIN TRY
			MERGE EmpresaServicio AS tar
			USING (SELECT @IdEmpresa IdEmpresa,t0.[value] IdServicio FROM dbo.FnSplit(@servicios) t0) as sou ON (tar.IdEmpresa = sou.IdEmpresa AND tar.IdServicio = sou.IdServicio)
			WHEN MATCHED THEN UPDATE SET tar.Activo = 1
			WHEN NOT MATCHED BY TARGET THEN INSERT(IdEmpresa,IdServicio) VALUES (sou.IdEmpresa,sou.IdServicio)
			WHEN NOT MATCHED BY SOURCE AND tar.IdEmpresa = @IdEmpresa THEN UPDATE SET tar.Activo = 0; 
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE();
		END CATCH
	end
END
