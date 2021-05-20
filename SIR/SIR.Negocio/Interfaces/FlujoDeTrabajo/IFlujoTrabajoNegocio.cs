using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.FlujoDeTrabajo
{
    public interface IFlujoTrabajoNegocio
    {
        MODResultado Ejecutar(MODFlujoFiltro filtro);
        MODResultado EditarCategoria(MODCategoria categoria);
        MODResultado CrearCategoria(MODCategoria categoria);
        MODResultado Registrar(MODFlujo registro);
        List<MODFlujo> Obtener(MODFlujoFiltro filtro);
        MODResultado ProbarConeccion(MODOrigenDatos origen);
        List<IDictionary<string, object>> Consultar(MODFlujoFiltro filtro);
        IEnumerable<MODGruposDeEjecucion> ObtenerPasos(MODFlujoFiltro filtro);
        MODResultado CrearFlujo(MODFlujo mod);
        List<MODGruposDeEjecucion> ObtenerGrupos(int idCategoria);
        List<MODGruposDeEjecucion> ObtenerGruposEjecucion(int prmIdCategoria);
        Dictionary<int, string> ObtenerVersiones(MODFlujoFiltro filtro);
        List<object> ObtenerOrigenes(MODFlujoFiltro filtro);
        List<MODOpciones> ObtenerOpciones(MODFlujoFiltro filtro);
        List<MODOpciones> ObtenerAcciones();
        List<object> ObtenerOrigenesPor(MODFlujoFiltro filtro);
        MODResultado GuardarConf(List<Dictionary<string, object>> lista);
        List<MODOpciones> ObtenerAccionesSeleccionadas(MODFlujoFiltro filtro);
        MODResultado GuardarCorrecciones(List<Dictionary<string, object>> lista);
        MODResultado ConfirmarCorrecciones(List<Dictionary<string, object>> lista);
    }
}
