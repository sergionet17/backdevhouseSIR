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
    public class CS8 : FormatoSUI
    {
        public override void Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "CS8", IdEmpresa = idEmpresa, IdServicio = idServicio });
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 8,
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
                Descripcion = "EXTR_CS8",
                Nombre = "EXTR_CS8",
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
                        Nombre = "niv_tension",
                        nombreVisible = "niv_tension",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
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
                        Nombre = "nro_semana",
                        nombreVisible = "nro_semana",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "5,0",
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
                        Nombre = "empresa",
                        nombreVisible = "empresa",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "id_mercado",
                        nombreVisible = "id_mercado",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 3,
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
                Descripcion = "Reporte CS8 - DECRETO ## - YYYY",
                Nombre = "CS8",
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
                        Nombre = "NRO_SEMANA",
                        nombreVisible = "NRO_SEMANA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "5,0",
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
                        Nombre = "NIV_TENSION",
                        nombreVisible = "NIV_TENSION",
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
                        Nombre = "PPST_95",
                        nombreVisible = "PPST_95",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PPST_99",
                        nombreVisible = "PPST_99",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PTHDV_95",
                        nombreVisible = "PTHDV_95",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PTHDV_99",
                        nombreVisible = "PTHDV_99",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PV2_V1_95",
                        nombreVisible = "PV2_V1_95",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PV2_V1_99",
                        nombreVisible = "PV2_V1_99",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PNHT",
                        nombreVisible = "PNHT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PNET",
                        nombreVisible = "PNET",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PNIT",
                        nombreVisible = "PNIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,4",
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
                        Nombre = "PERIODO",
                        nombreVisible = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
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
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 13,
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
