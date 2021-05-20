using SIR.Comun.Entidades;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectoPruebas.Preparacion_de_ambiente
{
    public static class ConfiguracionGeneral
    {
        private const int idUsuario = 27;
        private const string ip = "::1";
        private const string usuario = "jatencia";

        private static IEmpresasNegocio contextEmpresa = FabricaNegocio.CrearEmpresaNegocio;
        private static IServicioNegocio contextServio = FabricaNegocio.CrearServicioNegocio;

        public static void CrearEmpresa()
        {
            var resultado = contextEmpresa.CrearEmpresa(new MODEmpresa { 
                IdUsuario = idUsuario,
                Ip = ip,
                Usuario = usuario,

                IdEmpresa = 0,
                Activo = true,
                Correo = "celcia.informacion@correo.co",
                RazonSocial = "Celsia",
                Descripcion = "Somos los aliados energéticos de nuestros clientes, los asesoramos y acompañamos en sus necesidades de eficiencia energética, aprovechando sus recursos y generando ahorro de energía para la productividad de sus empresas, la sostenibilidad de sus ciudades y el bienestar de sus hogares. Nuestra estrategia está basada en la diferenciación, la innovación y la excelencia en el servicio.",
                FechaCreacion = DateTime.Now,
                NumeroIdentificacion = "888995562"
            });
            Console.WriteLine(@"Creacion de empresas, resultado: {0}", resultado.esValido);
        }
        public static IEnumerable<MODEmpresa> ConsultarEmpresa()
        {
            var resultado = contextEmpresa.ObtenerEmpresas();
            
            return resultado;
        }

        public static void CrearServico()
        {
            var resultado = contextServio.CrearServicio(new MODServicio
            {
                IdUsuario = idUsuario,
                Ip = ip,
                Usuario = usuario,

                IdServicio = 0,
                Nombre = "Energía",
                Descripcion = "Servicio publico de distribución de energía eléctrica",
                Empresas = ConsultarEmpresa()
            });
            Console.WriteLine(@"Creacion de servicos, resultado: {0}", resultado.esValido);
        }
    }
}
