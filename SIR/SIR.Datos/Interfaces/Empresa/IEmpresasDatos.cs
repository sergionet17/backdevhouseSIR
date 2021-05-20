using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Empresa
{
    public interface IEmpresaDatos
    {
        IEnumerable<MODEmpresa> ObtenerEmpresas();
        MODEmpresa ObtenerEmpresaPorId(int idEmpresa);
        MODResultado CrearEmpresa(MODEmpresa empresa);
        MODResultado ActualizarEmpresa(MODEmpresa empresa);
        MODResultado BorrarEmpresa(int idEmpresa);
    }
}
