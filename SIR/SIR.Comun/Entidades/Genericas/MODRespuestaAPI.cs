using SIR.Comun.Entidades.Abstracto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Comun.Entidades.Genericas
{
    [DataContract]
    [Serializable]
    public class MODRespuestaAPI<T> : BaseRespuesta
    {
        [DataMember]
        public T Resultado { get; set; }

        public MODRespuestaAPI(HttpStatusCode prmCodigoRespuesta, string prmCodigoMensaje)
        {
            base.CodigoRespuesta = prmCodigoRespuesta;
            base.CodigoMensaje = prmCodigoMensaje;
        }

        public MODRespuestaAPI(T prmModelo)
        {
            base.CodigoRespuesta = HttpStatusCode.OK;
            base.CodigoMensaje = "RTA000";
            this.Resultado = prmModelo;
        }

        public MODRespuestaAPI(T prmModelo, string prmCodigoMensaje)
        {
            base.CodigoRespuesta = HttpStatusCode.OK;
            base.CodigoMensaje = prmCodigoMensaje;
            this.Resultado = prmModelo;
        }
    }
}
