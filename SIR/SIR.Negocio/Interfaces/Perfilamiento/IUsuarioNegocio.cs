using SIR.Comun.Entidades.UsuarioPerfil;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIR.Negocio.Interfaces.Perfilamiento
{
    public interface IUsuarioNegocio
    {
        MODUsuario ObtenerUsuario(string prmUsername);

        bool ActualizarUltimaFechaLogin(string prmUsername);

        int CrearUsuario(MODUsuario prmUsuario, List<MODPermisoUsuario> prmPermisos);

        List<MODUsuarioBasico> ObtenerUsuarios(MODUsuarioFiltro filtro);

        bool ActualizarUsuario(MODUsuario prmUsuario, List<MODPermisoUsuario> prmPermisos);

        bool EliminarUsuario(int prmIdUsuario);

        Task<Tuple<bool, string>> ValidarUsuarioServicioExterno(string prmUsuario, string prmPassword, bool prmValidarExiste = false);

        MODUsuario ObtenerUsuarioPorId(int prmIdUsuario);
    }
}
