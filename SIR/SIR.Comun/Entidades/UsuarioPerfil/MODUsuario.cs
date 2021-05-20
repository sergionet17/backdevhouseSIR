using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    [DataContract]
    [Serializable]
    public class MODUsuario : MODUsuarioBasico
    {
        [DataMember]
        public List<MODPermisoUsuario> PermisosUsuario { get; set; }
        [DataMember]
        public List<MODPermisoReporte> PermisosReportes { get; set; }

        public MODUsuario()
        {
            this.PermisosUsuario = new List<MODPermisoUsuario>();
            this.PermisosReportes = new List<MODPermisoReporte>();
        }
    }
}