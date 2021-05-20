using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Comun.Entidades.Abstracto
{
    public abstract class MODArchivoBase
    {
        public int IdTipoArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdArchivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}
