using System;
using System.Net;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Abstracto
{
    [DataContract]
    [Serializable]
    public abstract class BaseRespuesta
    {
        [DataMember]
        public HttpStatusCode CodigoRespuesta { get; set; }
        [DataMember]
        public string CodigoMensaje { get; set; }
    }
}
