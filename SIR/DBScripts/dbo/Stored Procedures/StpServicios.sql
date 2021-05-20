-- =============================================
-- Author:		Carlos Torres
-- Create date: 21-02-2020
-- Description:	CRUD de Servicios
-- =============================================
CREATE PROCEDURE [dbo].[StpServicios]
	-- Add the parameters for the stored procedure here
	@accion int = 0, 
	@IdServicio	int= 0, 
	@Descripcion varchar(500) ='',
	@Nombre varchar(50) = '',
	@empresas varchar(3000) = ''
AS
BEGIN
	DECLARE @ids table(id INT);

	if @accion = 1
	begin 
		select t0.*,t2.*
		from Servicio t0
		LEFT JOIN EmpresaServicio t1 ON t0.IdServicio = t1.IdServicio AND t1.Activo = 1
		LEFT JOIN Empresa t2 ON t1.IdEmpresa = t2.IdEmpresa
	end
	else if @accion = 2
	begin
		select t0.*,t2.*
		from Servicio t0
		LEFT JOIN EmpresaServicio t1 ON t0.IdServicio = t1.IdServicio AND t1.Activo = 1
		LEFT JOIN Empresa t2 ON t1.IdEmpresa = t2.IdEmpresa
		where t0.IdServicio = @IdServicio
	end
	else if @accion = 3
	begin
		BEGIN TRY
				insert into Servicio(Descripcion,Nombre)
				OUTPUT inserted.IdServicio INTO @ids
				values(@Descripcion,@Nombre)				
				
				SELECT id
				FROM @ids
			
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE()
		END CATCH
	end
	else if @accion = 4
	begin
		BEGIN TRY
				UPDATE Servicio SET Descripcion = @Descripcion,Nombre = @Nombre
				WHERE IdServicio = @IdServicio						
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE()
		END CATCH
	end
	else if @accion = 5
	begin
		BEGIN TRY
			delete from EmpresaServicio where IdServicio = @IdServicio
			delete from Servicio where IdServicio = @IdServicio
		END TRY
		BEGIN CATCH
			SELECT N'No se puede borrar este servicio ya que tiene relaciones con otros elementos';
		END CATCH
	end
	else if @accion = 6
	begin		
		BEGIN TRY
			MERGE EmpresaServicio AS tar
			USING (SELECT @IdServicio IdServicio,t0.[value] IdEmpresa FROM dbo.FnSplit(@empresas) t0) as sou ON (tar.IdEmpresa = sou.IdEmpresa AND tar.IdServicio = sou.IdServicio)
			WHEN MATCHED THEN UPDATE SET tar.Activo = 1
			WHEN NOT MATCHED BY TARGET THEN INSERT(IdEmpresa,IdServicio) VALUES (sou.IdEmpresa,sou.IdServicio)
			WHEN NOT MATCHED BY SOURCE AND tar.IdServicio = @IdServicio THEN UPDATE SET tar.Activo = 0; 	
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE()
		END CATCH
		
	end

END
