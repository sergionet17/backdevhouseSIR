using SIR.Comun.Entidades.Abstracto;
using System;
using System.IO;

namespace SIR.Comun.Entidades.Archivos
{
    public class MODLogGeneracionArchivo: MODArchivoBase
    {
        public MODLogGeneracionArchivo()
        {
            RutaDestino = string.Empty;
            FechaCreacion = DateTime.Now;
        }
        public long IdLog { get; set; }
        public int? IdFlujo { get; set; }
        public string FlujoTrabajo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string RutaDestino { get; set; }
        public string NombreArchivo { get; set; }
        public byte[] Contenido { get; set; }
    }
}
