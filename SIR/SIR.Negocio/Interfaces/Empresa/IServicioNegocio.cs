using SIR.Comun;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Empresas;
using SIR.Comun.Entidades.Genericas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Negocio.Interfaces.Empresa
{
    public interface IServicioNegocio
    {
        IEnumerable<MODServicio> ObtenerServicios();
        MODServicio ObtenerServicioPorId(int idServicio);
        MODResultado CrearServicio(MODServicio servicio);
        MODResultado ActualizarServicio(MODServicio servico);
        List<MODEmpresaServicio> ObtenerServiciosPorEmpresa(int prmIdEmpresa);
        MODResultado BorrarServicio(MODServicio servicio);
    }
}
