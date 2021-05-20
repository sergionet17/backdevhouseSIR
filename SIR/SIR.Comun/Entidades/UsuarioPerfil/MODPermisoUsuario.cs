using SIR.Comun.Enumeradores;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    [DataContract]
    [Serializable]
    public class MODPermisoUsuario
    {
        [DataMember]
        public int IdUsuario { get; set; }
        [DataMember]
        public int? IdModuloEmpresaServicio { get; set; }
        [DataMember]
        public int? IdReporteEmpresaServicio { get; set; }
        [DataMember] 
        public int IdEmpresa { get; set; }
        [DataMember] 
        public int IdServicio { get; set; }
        [DataMember] 
        public int IdModulo { get; set; }
        [DataMember] 
        public EnumTipoPermiso TipoPermiso { get; set; }
        [DataMember] 
        public bool Activo { get; set; }
    }
}
