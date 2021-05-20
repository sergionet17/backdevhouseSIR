using SIR.Comun.Entidades.UsuarioPerfil;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Perfilamiento
{
    public interface IUsuarioDatos
    {
        MODUsuario ObtenerUsuario(string prmUsername);

        bool ActualizarUltimaFechaLogin(string prmUsername);

        int CrearUsuario(MODUsuario prmUsuario);

        bool InsertarPermisosUsuario(List<MODPermisoUsuario> prmPermisos);

        List<MODUsuarioBasico> ObtenerUsuarios(MODUsuarioFiltro filtro);

        bool ActualizarUsuario(MODUsuario prmUsuario);
        bool EliminarUsuario(int prmIdUsuario);
        public MODUsuario ObtenerUsuarioPorId(int prmIdUsuario);
    }
}
