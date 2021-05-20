using SIR.Comun.Entidades.Abstracto;
using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    [DataContract]
    [Serializable]
    public class MODUsuarioFiltro : MODBase
    {
        [DataMember]
        public int? IdEmpresa { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Apellido { get; set; }
        [DataMember]
        public int? Desde { get; set; }
        [DataMember]
        public int? Hasta { get; set; }
        [DataMember]
        public bool? Activo { get; set; }
    }
}