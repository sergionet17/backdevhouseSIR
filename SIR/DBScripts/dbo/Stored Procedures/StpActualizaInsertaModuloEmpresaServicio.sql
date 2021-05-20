CREATE procedure [dbo].[StpActualizaInsertaModuloEmpresaServicio]
(
	  @prmIdServicio int
	, @prmIdEmpresa int
	, @prmIdModulo int
	, @prmActivo bit
	, @prmIdModuloEmpresaServicio int output
)
as
begin
	declare @IdEmpresaServicio int = 0
	select @IdEmpresaServicio = IdEmpresaServicio from dbo.EmpresaServicio with(nolock) where IdEmpresa = @prmIdEmpresa and IdServicio = @prmIdServicio

	if @IdEmpresaServicio <> 0
	begin
		if exists(select IdModuloEmpresaServicio from dbo.ModuloEmpresaServicio with(nolock) where IdEmpresaServicio = @IdEmpresaServicio and IdModulo = @prmIdModulo)
		begin
			update dbo.ModuloEmpresaServicio
			set Activo = @prmActivo
			where IdEmpresaServicio = @IdEmpresaServicio and IdModulo = @prmIdModulo
			select @prmIdModuloEmpresaServicio = IdModuloEmpresaServicio from dbo.ModuloEmpresaServicio with(nolock) where IdEmpresaServicio = @IdEmpresaServicio and IdModulo = @prmIdModulo
		end
		else
		begin
			insert into dbo.ModuloEmpresaServicio(IdEmpresaServicio, IdModulo, Activo)
			values(@IdEmpresaServicio, @prmIdModulo, @prmActivo)
			select @prmIdModuloEmpresaServicio = SCOPE_IDENTITY()
		end
	end

end