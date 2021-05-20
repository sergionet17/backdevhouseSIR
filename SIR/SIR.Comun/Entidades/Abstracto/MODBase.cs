using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Abstracto{
    [Serializable]
    [DataContract]
    public class MODBase{
        [DataMember]
        public int IdUsuario { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public string Ip { get; set; }
        public MODBase.NotificarCliente Notificar;
        public delegate void NotificarCliente(string usuario, string mensaje);
    }
}