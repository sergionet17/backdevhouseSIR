using ProjectoPruebas.Interfaces;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoPruebas.Formatos
{
    public class CS7 : FormatoSUI
    {
        public override void Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "CS7", IdEmpresa = idEmpresa, IdServicio = idServicio });
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 7,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = reporte.FirstOrDefault(rep => rep.EsReporte).Id,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 0,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 7
            });
        }
        public override void Estructuras()
        {
            MODReporte estructura = new MODReporte()
            {
                Descripcion = "EXTR_CS7",
                Nombre = "EXTR_CS7",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                //ToDo esta tabla tiene pendiente obtner informacion de TBL_PAR_CALIDAD_PERIODOS, TBL_PUNTOS_MEDIDA y EXTR_CS7_PME
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_P_MEDIDA_LIN",
                        nombreVisible = "COD_P_MEDIDA_LIN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NRO_SEMANA",
                        nombreVisible = "NRO_SEMANA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHT",
                        nombreVisible = "NHT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHIT",
                        nombreVisible = "NHIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHMT",
                        nombreVisible = "NHMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHTT",
                        nombreVisible = "NHTT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NET",
                        nombreVisible = "NET",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NEIT",
                        nombreVisible = "NEIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NEMT",
                        nombreVisible = "NEMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NETT",
                        nombreVisible = "NETT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIT",
                        nombreVisible = "NIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIMT",
                        nombreVisible = "NIMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NITT",
                        nombreVisible = "NITT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 12,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NFPC",
                        nombreVisible = "NFPC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 13,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NFPI",
                        nombreVisible = "NFPI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 14,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 15,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 16,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);


            estructura = new MODReporte()
            {
                Descripcion = "Reporte CS7 - DECRETO ## - YYYY",
                Nombre = "CS7",
                IdCategoria = idCategoriaSUI,
                EsReporte = true,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_P_MEDIDA_LIN",
                        nombreVisible = "COD_P_MEDIDA_LIN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NRO_SEMANA",
                        nombreVisible = "NRO_SEMANA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHT",
                        nombreVisible = "NHT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHIT",
                        nombreVisible = "NHIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHMT",
                        nombreVisible = "NHMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NHTT",
                        nombreVisible = "NHTT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NET",
                        nombreVisible = "NET",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NEIT",
                        nombreVisible = "NEIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NEMT",
                        nombreVisible = "NEMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NETT",
                        nombreVisible = "NETT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIT",
                        nombreVisible = "NIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIMT",
                        nombreVisible = "NIMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NITT",
                        nombreVisible = "NITT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 12,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NFPC",
                        nombreVisible = "NFPC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 13,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NFPI",
                        nombreVisible = "NFPI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
                        Ordinal = 14,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 15,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 16,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "PERIODO",
                        nombreVisible = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 17,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);
        }
        public override void Flujo()
        {
            base.Flujo();
        }
        public override void Generar(bool crear = false)
        {
            base.Generar(crear);
        }
        public override void Inicializar()
        {
            base.Inicializar();
        }
        public override IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return base.ObtenerEstructuras();
        }
    }
}
