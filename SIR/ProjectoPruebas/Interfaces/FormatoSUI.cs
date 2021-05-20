using SIR.Comun.Entidades.Reportes;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Archivos;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using SIR.Negocio.Interfaces.Reporte;
using System.Collections.Generic;

namespace ProjectoPruebas.Interfaces
{
    public abstract class FormatoSUI
    {
        #region campos
        public int idUsuario = 27;
        public string ip = "::1";
        public string usuario = "jatencia";

        public int idEmpresa;
        public int idServicio;
        public int idCategoriaSUI = 2;//Esto se obtine de la tabla [dbo].[CategoriasFlujos]
        public int grupoEjecucionEstraccion = 12;//Esto se obtine de la tabla [dbo].[GruposDeEjecucion] -- Es configuracion interna.
        public int grupoEjecuciontranformacion = 13;//Esto se obtine de la tabla [dbo].[GruposDeEjecucion] -- Es configuracion interna.

        public IReporteNegocio contextReporte = FabricaNegocio.CrearReporteNegocio;
        public IFlujoTrabajoNegocio contextFlujo = FabricaNegocio.CrearFlujoTrabajoNegocio;
        public IArchivoNegocio ContextArchivo = FabricaNegocio.CrearArchivoNegocio;
        #endregion

        //public abstract void Archivos();
        

        public virtual void Ejecutar()
        {
            
        }

        public virtual void Estructuras()
        {
            
        }

        public virtual void Flujo()
        {
            
        }

        public virtual void Generar(bool crear = false)
        {
           
        }

        public virtual void Inicializar()
        {
            
        }

        public virtual IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return contextReporte.Obtener(new MODReporteFiltro());
        }
    }
}
