using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Empresas;
using SIR.Comun.Entidades.Genericas;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Empresa
{
    public interface IServicioDatos{
        IEnumerable<MODServicio> ObtenerServicios();
        MODServicio ObtenerServicioPorId(int idServicio);
        MODResultado CrearServicio(MODServicio servicio);
        MODResultado ActualizarServicio(MODServicio servico);
        List<MODEmpresaServicio> ObtenerServiciosPorEmpresa(int prmIdEmpresa);
        MODResultado BorrarServicio(int idServicio);
    }
}