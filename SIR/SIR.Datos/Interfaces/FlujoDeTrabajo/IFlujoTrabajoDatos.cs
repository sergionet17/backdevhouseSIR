using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Enumeradores;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.FlujoDeTrabajo
{
    public interface IFlujoTrabajoDatos
    {
        List<MODFlujo> ObtenerFlujos();
        MODResultado Historico(ref MODFlujoHistorico registro, EnumAccionBaseDatos enumAccion);
        List<MODCategoria> ObtenerCategorias();
        MODResultado EditarCategoria(MODCategoria categoria);
        MODResultado CrearCategoria(MODCategoria categoria);
        MODResultado Registrar(MODFlujo registro);
        MODResultado ProbarConeccion(MODOrigenDatos origen);
        List<IDictionary<string, object>> Consultar(List<MODCampos> campos, string tabla, Dictionary<string, object> parametros);
        IEnumerable<MODGruposDeEjecucion> ObtenerPasos(MODFlujoFiltro filtro);
        List<IDictionary<string, object>> EjecutarScirpt(List<MODCampos> campos, string consulta, Dictionary<string, object> parametros);
        List<MODGruposDeEjecucion> ObtenerGruposEjecucion();
        List<MODGruposDeEjecucion> ObtenerGruposEjecucion(int prmIdCategoria);
        Dictionary<int, string> ObtenerVersiones(MODFlujoFiltro filtro);
        List<MODOpciones> ObtenerOpciones(MODFlujoFiltro filtro);
        void GenerarTablaTemporal(MODTarea tarea);
        List<string> conPrerequisito(MODFlujoFiltro filtro);
        List<MODOpciones> ObtenerAcciones();
        MODResultado GuardarConf(List<Dictionary<string, object>> lista);
        List<MODOpciones> ObtenerAccionesSeleccionadas(MODFlujoFiltro filtro);
        MODResultado GuardarCorrecciones(string sql,List<Dictionary<string, object>> lista);
        MODResultado ConfirmarCorrecciones(string sql,List<Dictionary<string, object>> lista);
    }
}
