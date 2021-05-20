using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    [DataContract]
    public class MODUsuarioBasico
    {
        [DataMember]
        public int IdUsuario { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Apellido { get; set; }
        [DataMember]
        public DateTime FechaRegistro { get; set; }
        [DataMember]
        public DateTime FechaUltimoLogin { get; set; }
        [DataMember]
        public DateTime FechaModificacion { get; set; }
        [DataMember]
        public bool EsAdministrador { get; set; }
        [DataMember]
        public bool Activo { get; set; }
    }
}
