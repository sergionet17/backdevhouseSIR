using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Genericas;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.Empresa
{
    public interface IEmpresasNegocio
    {
        IEnumerable<MODEmpresa> ObtenerEmpresas();
        MODEmpresa ObtenerEmpresaPorId(int idEmpresa);
        MODResultado CrearEmpresa(MODEmpresa empresa);
        MODResultado ActualizarEmpresa(MODEmpresa empresa);
        MODResultado BorrarEmpresa(MODEmpresa empresa);
    }
}
