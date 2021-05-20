using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Empresa;
using SIR.Datos.Interfaces.FlujoDeTrabajo;
using SIR.Datos.Interfaces.Reporte;
using SIR.Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos
{
    public class ConfiguracionNegocio
    {
        #region Singleton
        private static volatile ConfiguracionNegocio _Instancia;
        private static readonly object _syncRoot = new object();

        public static ConfiguracionNegocio Intancia { get { lock (_syncRoot) { if (_Instancia == null) { _Instancia = new ConfiguracionNegocio(); } } return _Instancia; } }
        #endregion
        private ConfiguracionNegocio()
        {
            ConfiguracionAplicacion();
        }
        /**************campos****************/
        private static List<MODServicio> _servicios;
        private static List<MODEmpresa> _empresas;
        private static List<MODFlujo> _flujos;
        private static List<MODReporte> _reportes;
        private static List<MODReporte> _reportesLimpio;
        private static List<MODCategoria> _categoriasFlujos;
        private static List<MODGruposDeEjecucion> _gruposEjecucion;
        private static List<MODOpciones> _acciones;

        /**************propiedades***********/
        public static List<MODServicio> Servicios { get {
                if (_servicios == null)
                {
                    IServicioDatos idatos = FabricaDatos.CrearServicioDatos;
                    _servicios = idatos.ObtenerServicios().ToList();
                }
                return _servicios;
            } private set { _servicios = value; }  }
        public static List<MODEmpresa> Empresas { get{
                if (_empresas == null)
                {
                    IEmpresaDatos idatos = FabricaDatos.CrearEmpresaDatos;
                    _empresas = idatos.ObtenerEmpresas().ToList();
                }
                return _empresas;
            } private set { _empresas = value; } }

        public static List<MODFlujo> Flujos
        {
            get
            {
                if (_flujos == null)
                {
                    IFlujoTrabajoDatos idatos = FabricaDatos.CrearFlujoTrabajoDatos;
                    _flujos = idatos.ObtenerFlujos();
                }
                return _flujos;
            }
            private set { _flujos = value; }
        }

        private static List<MODLoginUsuario> _logueado;
        public static List<MODLoginUsuario> Logueos
        {
            get
            {
                return _logueado?? new List<MODLoginUsuario>();
            }
            private set { _logueado = value; }
        }

        public static List<MODReporte> Reportes
        {
            get
            {
                if (_reportes == null)
                {
                    IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
                    _reportes = iReporte.ObtenerReportes().ToList();
                }
                return _reportes;
            }
            private set { _reportes = value; }
        }

        public static List<MODCategoria> CategoriasFlujos { 
            get {  
                if(_categoriasFlujos == null){
                    IFlujoTrabajoDatos iFTD = FabricaDatos.CrearFlujoTrabajoDatos;
                    _categoriasFlujos = iFTD.ObtenerCategorias();
                }
                return _categoriasFlujos;
            }  
            private set{
                _categoriasFlujos = value;
            } 
        }

        public static List<MODGruposDeEjecucion> GruposDeEjecucion{
            get{
                if(_gruposEjecucion == null){
                    IFlujoTrabajoDatos datos = FabricaDatos.CrearFlujoTrabajoDatos;
                    _gruposEjecucion = datos.ObtenerGruposEjecucion();
                }
                return _gruposEjecucion;
            }
            private set{
                _gruposEjecucion = value;
            }
        }
        public static List<MODReporte> ReportesLimpio
        {
            get
            {
                if (_reportesLimpio == null)
                {
                    IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
                    _reportesLimpio = iReporte.ObtenerReportesLimpio().ToList();
                }
                return _reportesLimpio;
            }
            private set { _reportesLimpio = value; }
        }
        public static List<MODOpciones> Acciones{
            get{
                if(_acciones == null){
                    IFlujoTrabajoDatos flujoDatos = FabricaDatos.CrearFlujoTrabajoDatos;
                    _acciones = flujoDatos.ObtenerAcciones();
                }
                return _acciones;
            }
            private set {_acciones = value;}
        }
        /**************metodos****************/
        private void ConfiguracionAplicacion()
        {
            IServicioDatos iServcio = FabricaDatos.CrearServicioDatos;
            _servicios = iServcio.ObtenerServicios().ToList();

            IEmpresaDatos iEmpresa = FabricaDatos.CrearEmpresaDatos;
            _empresas = iEmpresa.ObtenerEmpresas().ToList();

            IFlujoTrabajoDatos iFlujo = FabricaDatos.CrearFlujoTrabajoDatos;
            _flujos = iFlujo.ObtenerFlujos();

            IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
            _reportes = iReporte.ObtenerReportes().ToList();
            _reportesLimpio = iReporte.ObtenerReportesLimpio().ToList();
        }

        public static void RefrescarConfiguracion(EnumTipoConfiguracion tipoConfiguracion)
        {
            IEmpresaDatos configuracion = FabricaDatos.CrearEmpresaDatos;
            IServicioDatos sd = FabricaDatos.CrearServicioDatos;
            switch (tipoConfiguracion)
            {
                case EnumTipoConfiguracion.servicios:
                    Servicios = sd.ObtenerServicios().ToList();
                    break;
                case EnumTipoConfiguracion.empresas:
                    Empresas = configuracion.ObtenerEmpresas().ToList();
                    break;
                case EnumTipoConfiguracion.flujo:
                    IFlujoTrabajoDatos idatos = FabricaDatos.CrearFlujoTrabajoDatos;
                    _flujos = idatos.ObtenerFlujos();
                    break;
                case EnumTipoConfiguracion.reportes:
                    IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
                    _reportes = iReporte.ObtenerReportes().ToList();
                    break;
                case EnumTipoConfiguracion.categoriasFlujos:
                    IFlujoTrabajoDatos datos = FabricaDatos.CrearFlujoTrabajoDatos;
                    CategoriasFlujos = datos.ObtenerCategorias();
                    break;
                default:
                    break;
            }
        }

        public static bool SessionUsuario(MODLoginUsuario registro, EnumSession peticion)
        {
            List<MODLoginUsuario> actual = new List<MODLoginUsuario>();
            actual.AddRange(Logueos);
            switch (peticion)
            {
                case EnumSession._inicio:
                    int _tiempo = Convert.ToInt32(Configuraciones.ObtenerConfiguracion("Contexto", "TiempoSession"));
                    if ((Logueos.Any(y => y.UserName == registro.UserName && (DateTime.Now - y.Fecha).TotalMinutes > _tiempo)) ||
                         (Logueos.Any(y => y.UserName == registro.UserName && y.IP == registro.IP)))
                    {
                        registro.Fecha = DateTime.Now;
                        actual.Remove(Logueos.FirstOrDefault(y => y.UserName == registro.UserName));
                        actual.Add(registro);
                    }
                    else if (!Logueos.Any(y => y.UserName == registro.UserName))
                    {
                        registro.Fecha = DateTime.Now;
                        actual.Add(registro);
                    }else { return false; }
                    break;
                case EnumSession._peticion:
                    if (Logueos.Any(y => y.UserName == registro.UserName))
                    {
                        actual.FirstOrDefault(y => y.UserName == registro.UserName).Fecha = DateTime.Now;
                    }
                    else { return false; }
                    break;
                case EnumSession._cierre:
                    Logueos.Remove(Logueos.FirstOrDefault(y => y.UserName == registro.UserName));
                    break;
                default:
                    break;
            }
            ConfiguracionNegocio.Logueos = actual;
            return true;
        }
    }
}
