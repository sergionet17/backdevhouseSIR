using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Parametrizacion_SIR
{
    [DataContract]
    [Serializable]
    public class MOD_Cliente_ExcluidoFiltro
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string NUI { get; set; }
        [DataMember]
        public int Id_Causa { get; set; }
        [DataMember]
        public int Usuario_Mod { get; set; }
    }
}
