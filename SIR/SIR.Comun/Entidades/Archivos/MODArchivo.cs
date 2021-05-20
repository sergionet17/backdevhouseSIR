using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.IO;

namespace SIR.Comun.Entidades.Archivos
{
    public class MODArchivo : MODArchivoBase
    {
        public Stream Contenido { get; set; }
        public int IdReporte { get; set; }
        public string Reporte { get; set; }
        public int IdSeparador { get; set; }
        public string Extension { get; set; }
        public string Separador { get; set; }
        public string ValorSeparador { get; set; }
        public int IdTarea { get; set; }
        public DateTime Periodo { get; set; }
        public EnumPeriodicidad Periodicidad { get; set; }
        public int DatoPeriodo { get; set; }
        public int? IdFlujo { get; set; }
        public int? IdElementoFlujo { get; set; }
        public List<MODCamposArchivo> Campos { get; set; }
        public bool QuitarCabesera { get; set; }

        public MODArchivo()
        {
            this.Campos = new List<MODCamposArchivo>();
        }
    }
}
