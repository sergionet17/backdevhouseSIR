using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Reporte;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using ProjectoPruebas.Preparacion_de_ambiente;
using System.Collections.Generic;
using System.Linq;
using SIR.Negocio.Interfaces.Archivos;
using SIR.Negocio.Concretos;

namespace ProjectoPruebas
{
    public static class LacDiario
    {
        private const int idUsuario = 27;
        private const string ip = "::1";
        private const string usuario = "jatencia";

        private static int idEmpresa;
        private static int idServicio;
        private static int idCategoriaSUI = 1;//Esto se obtine de la tabla [dbo].[CategoriasFlujos]
        
        private static IReporteNegocio contextReporte = FabricaNegocio.CrearReporteNegocio;
        private static IFlujoTrabajoNegocio contextFlujo = FabricaNegocio.CrearFlujoTrabajoNegocio;
        private static IArchivoNegocio contexArchivo = FabricaNegocio.CrearArchivoNegocio;

        static void Inicializar()
        {
            var empresas = ConfiguracionGeneral.ConsultarEmpresa();
            idEmpresa = empresas.FirstOrDefault(x=>x.RazonSocial == "CETSA").IdEmpresa;
            idServicio = empresas.FirstOrDefault(x => x.RazonSocial == "CETSA").Servicios.FirstOrDefault().IdServicio;
        }
        static IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return contextReporte.Obtener(new MODReporteFiltro());
        }
        static IEnumerable<MODArchivo> ObtenerArchivos() {
            return contexArchivo.ObtenerArchivos(new MODArchivoFiltro { });
        }
        static void CrearReporteDiario()
        {
            var estructuras = ObtenerEstructuras();
            var archivos = ObtenerArchivos();
            MODReporte reporte = estructuras.FirstOrDefault(x => x.Nombre == "LAC_DIARIO");
            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //Energia
                IdEmpresa = idEmpresa, //Cetsa
                NombreEmpresa = "Cetsa",
                IdElemento = reporte.Id, // Diario
                Elemento = "Diario",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.diario,//Campo nuevo del reporte

                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.LAC,
                IdCategoria = (int)SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.LAC,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1
            };
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INCIDENCIAS");
            //Incidencia
            var consulta1 = @"SELECT a.codigo,   
           a.nombre nombre_circuito,
           d.nro_incidencia,
           b.nombre as subestacion,
           f.descripcion as sector,
           nvl(decode(a.usuarios,0, 1, a.usuarios), 1) as usuarios,
           1 as demanada,
           round(1/y.horas_mes, 3) potencia_prom,
           c.descripcion voltaje,
           a.grupo,
           a.km_subt + a.km_aerea longitud_red,
           a.rele_recierre,
           a.alimentador_radial,
           a.normal_abierto,
           a.longitud,
           a.latitud,
           a.altitud,
           a.propiedad,
           a.km_subt,
           a.km_aerea,
           a.empresa,
           replace(replace(replace(replace(replace(replace (d.descripcion, chr(13), ' '), chr(10), ' '), chr(124), ' '), chr(59), ' '), chr(44), ' '), chr(9), ' ') as DESCRIPCION,
           d.subfamilia_causa, 
           d.duracion,
           d.f_reposicion,
           d.f_alta,
           replace(replace(replace(replace(replace (replace (d.desc_incidencia, chr(13), ' '), chr(10), ' '), chr(124), ' '), chr(59), ' '), chr(44), ' '), chr(9), ' ') as desc_incidencia,
           d.causa_desc,
           replace(replace(replace(replace(replace (replace (D.observacion, chr(13), ' '), chr(10), ' '), chr(124), ' '), chr(59), ' '), chr(44), ' '), chr(9), ' ') as observacion,
           d.pot_afectada
    from   (SELECT (24*to_number(to_char(last_day(to_date(@periodo,'yyyymmdd')), 'dd'))) horas_mes 
            FROM   dual) y,
           (SELECT codigo, descripcion 
            FROM   sgd_maestro 
            WHERE  tip_tabla = 'NCMD') f,
           (SELECT codigo, nombre 
            FROM   bdiv10_sgd_subestac 
            WHERE  onis_stat = '0'
            AND    onis_ver not like '%.%') b,
           (SELECT codigo, rtrim(descripcion, ' KV') as descripcion 
            FROM   sgd_maestro 
            WHERE  tip_tabla = 'TNOM') c,
           (SELECT u.nro_incidencia,
                   u.nro_instalacion,
                   u.f_reposicion,
                   u.f_alta,
                   trunc(((u.f_reposicion - u.f_alta)*24*60),2) as duracion,
                   v.desc_incidencia,
                   v.observacion,
                   v.pot_afectada,
                   x.descripcion causa_desc,
                   w.DESCRIPCION,
                   T.DESCRIPCION subfamilia_causa
            FROM   gi_subtipos w, 
                   gi_subtipo_causa t, 
                   gi_causa x,
                   sgd_indisponibilidad u, 
                   sgd_incidencia v
            WHERE  v.nro_incidencia = u.nro_incidencia
            AND    ((u.f_reposicion >= to_date(@periodo||'00:00:00', 'yyyymmdd hh24:mi:ss')
                    AND    u.f_reposicion <= to_date(@periodo||'23:59:59', 'yyyymmdd hh24:mi:ss'))
                    or u.f_reposicion is null)
            AND    v.est_actual <> 11
            AND    v.alcance = 2
            AND    v.cod_causa = x.cod_causa
            AND    X.SUBTIPO   = w.SUBTIPO
            AND    t.SUBTIPO   = w.SUBTIPO
            AND    t.GPO_CAUSA = x.GPO_CAUSA
            AND    T.TIPO = X.TIPO) d,
           (SELECT k.codigo,
                   k.nro_centro,
                   k.nro_cmd,
                   k.nombre,
                   k.usuarios,
                   k.grupo,
                   k.longitud_red,
                   k.rele_recierre,
                   k.alimentador_radial,
                   k.normal_abierto,
                   k.longitud,
                   k.latitud,
                   k.altitud,
                   decode(n.propiedad_bdi, '', 'N', 0, 'N', 1, 'N', 7, 'N', 8,'N', 'S' ) propiedad,
                   nvl(k.km_subt,0) as km_subt,
                   nvl(k.km_aerea, 0) as km_aerea,
                   m.empresa,
                   k.instalacion_origen,
                   k.ten_nom
            FROM   TRSF_F4_5_PARIDAD_EMPRESAS m, 
                   TRSF_F4_5_PARIDAD_EMPRESAS n,
                   (SELECT codigo_sal 
                    FROM   EXTR_F4_ALIMENTADORES 
                    WHERE  activo = 'S') l,
                   (SELECT codigo, 
                           nombre, 
                           cant_clientes usuarios, 
                           grupo,
                           (nvl(km_subt, 0) + nvl(km_aerea, 0)) as longitud_red,
                           rele_recierre, 
                           alimentador_radial, 
                           normal_abierto, 
                           longitud,
                           latitud, 
                           altitud, 
                           to_number(propiedad) propiedad,
                           km_subt, 
                           km_aerea, 
                           nro_centro,
                           substr(instalacion_origen_v10,instr(instalacion_origen_v10, ':',1)+1) as instalacion_origen, 
                           ten_nom, 
                           0 as bdi_job, 
                           nro_cmd
                    FROM   bdiv10_sgd_salmt
                    WHERE  onis_stat = '0'
                    AND    onis_ver not like '%.%'
                    UNION ALL
                    SELECT codigo, 
                           nombre, 
                           cant_clientes usuarios, 
                           grupo,
                           (nvl(km_subt, 0) + nvl(km_aerea, 0)) as longitud_red,
                           rele_recierre, 
                           linea_radial as alimentador_radial,
                           normal_abierto, 
                           longitud,
                           latitud, 
                           altitud, 
                           to_number(propiedad) propiedad,
                           km_subt, 
                           km_aerea, 
                           nro_centro,
                           substr(instalacion_origen_v10,instr(instalacion_origen_v10, ':',1)+1) as instalacion_origen, 
                           ten_nom, 
                           0 as bdi_job, 
                           nro_cmd
                    FROM   bdiv10_sgd_salat
                    WHERE  onis_stat = '0'
                    AND    onis_ver not like '%.%') k
            WHERE k.codigo = l.codigo_sal
            AND   k.nro_centro = m.nro_centro_bdi
            AND   k.propiedad = n.propiedad_bdi(+)) a
    WHERE  a.instalacion_origen = b.codigo(+)
    AND    a.codigo = d.nro_instalacion
    AND    a.ten_nom = c.codigo(+)
    AND    a.nro_cmd = f.codigo(+)        
    AND    a.nro_cmd = f.codigo(+)";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "XMD_INCIDENCIAS",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta1,
                },
                IdGrupoEjecucion = 1,
            });
            //Interrucciones
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INTERRUPCIONES");
            consulta1 = @"SELECT  c.nro_incidencia,
            a.cod_circuito cod_circuito,
            a.NOMBRE descripcion_circuito,
            a.sector_circuito as sector,
            a.codigo,
            a.grupo,
            a.matricula_trafo,
            a.matricula_ct_apoyo,
            a.potencia_nominal capacidad,
            a.dem_energia as dem_energia,
            a.cant_clientes as cant_clientes,
            a.longitud,
            a.latitud,
            a.altitud,
            a.propiedad,
            a.empresa,
            a.tipo_ct,
            c.descripcion,
            c.duracion,
            c.f_reposicion,
            c.f_alta,
            c.causa_desc,
            c.subfamilia_causa,
            c.inc_agrupada,
            c.usuario,
            c.incidencia_manio_apert,
            c.incidencia_manio_cierre,
            c.observacion as observacion,
            c.descripcion_incidencia as desc_incidencia,
            a.codigo_ct as codigo_ct,
            c.f_actual
    FROM    (SELECT j.codigo,
                    j.matricula_trafo,
                    j.matricula_ct_apoyo,
                    j.grupo,
                    j.potencia_nominal,
                    0 as dem_energia,
                    j.cant_clientes,
                    l.descripcion propiedad,
                    j.empresa,
                    j.instalacion_origen,
                    j.nombre,
                    j.tipo_ct,
                    j.longitud,
                    j.latitud,
                    j.ALTITUD,
                    j.cod_circuito,
                    j.sector_circuito,
                    j.codigo_ct
             FROM   (SELECT codigo, descripcion
                     FROM   bdiv10_maestro
                     WHERE  tip_tabla = 'PROP') l,
                    (SELECT a.codigo,
                            b.codigo codigo_ct,
                            a.matricula as matricula_trafo,
                            b.matricula as matricula_ct_apoyo,
                            a.grupo,
                            a.potencia_nominal,
                            a.cant_clientes,
                            SUBSTR(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10,':', 1)+1) as instalacion_origen,
                            C.nombre,
                            M.nom_centro as empresa,
                            c.codigo as cod_circuito,
                            p.descripcion as sector_circuito,
                            A.NRO_CENTRO,
                            A.PROPIEDAD,
                            B.TIPO_CT,
                            b.longitud,
                            b.latitud,
                            B.ALTITUD
                      FROM  (SELECT codigo, descripcion FROM  bdiv10_maestro where tip_tabla = 'NMES') p,
                            sgd_centro m ,
                            bdiv10_sgd_salmt c,
                            bdiv10_sgd_ct b,
                            bdiv10_sgd_trafo_mb a
                      WHERE a.nro_centro = m.nro_centro(+)
                      AND   B.CODIGO = SUBSTR(A.INSTALACION_ORIGEN_V10,INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1)
                      AND   C.CODIGO = SUBSTR(B.INSTALACION_ORIGEN_V10,INSTR(B.INSTALACION_ORIGEN_V10, ':', 1)+1)
                      AND   c.nro_mesa = p.codigo(+)
                      AND   c.onis_stat = '0'
                      AND   c.onis_ver not like '%.%'
                      AND   b.onis_stat = '0'
                      AND   b.onis_ver not like '%.%'
                      AND   a.onis_stat = '0'
                      AND   a.onis_ver not like '%.%') j
             WHERE  j.propiedad = l.codigo(+)) a,
            (SELECT u.nro_incidencia,
                    u.nro_instalacion,
                    u.f_reposicion,
                    u.f_alta,
                    u.f_actual,
                    trunc((u.f_reposicion-u.f_alta)*24*60,2) duracion,
                    V.NRO_AGRUPACION as inc_agrupada,
                    x.descripcion causa_desc,
                    w.DESCRIPCION,
                    T.DESCRIPCION subfamilia_causa,
                    nvl(U.maniobra_apert_fase_a , nvl(U.maniobra_apert_fase_b , U.maniobra_apert_fase_c )) as incidencia_manio_apert,
                    nvl(U.maniobra_cierre_fase_a, nvl(U.maniobra_cierre_fase_b, U.maniobra_cierre_fase_c)) as incidencia_manio_cierre,
                    replace(replace(replace(replace(replace(replace(V.DESC_INCIDENCIA, chr(13), ' '), chr(10), ' '), chr(124), ' '), chr(59), ' '), chr(44), ' '), chr(9), ' ') as descripcion_incidencia,
                    replace(replace(replace(replace(replace(replace(V.OBSERVACION, chr(13), ' '), chr(10), ' '), chr(124), ' '), chr(59), ' '), chr(44), ' '), chr(9), ' ') as OBSERVACION,
                    u.usuario
             FROM   gi_causa x,
                    gi_subtipo_causa t,
                    gi_subtipos w,
                    sgd_interrupcion u,
                    sgd_incidencia v
             WHERE  v.nro_incidencia = u.nro_incidencia
             AND    (u.f_reposicion >= to_date(@periodo||' 00:00:00','yyyymmdd hh24:mi:ss') OR u.f_reposicion is null)
             and    u.f_alta<to_date( to_char( to_date(@periodo,'yyyymmdd')+1,'yyyymmdd'  )||' 00:00:00','yyyymmdd hh24:mi:ss')
             and ( ( (nvl(U.MANIOBRA_APERT_FASE_A,0)+nvl(U.MANIOBRA_CIERRE_FASE_A,0)>nvl(U.MANIOBRA_APERT_FASE_A,0)) or (nvl(U.MANIOBRA_APERT_FASE_A,0)+nvl(U.MANIOBRA_CIERRE_FASE_A,0)=0) ) and
                   ( (nvl(U.MANIOBRA_APERT_FASE_B,0)+nvl(U.MANIOBRA_CIERRE_FASE_B,0)>nvl(U.MANIOBRA_APERT_FASE_B,0)) or (nvl(U.MANIOBRA_APERT_FASE_B,0)+nvl(U.MANIOBRA_CIERRE_FASE_B,0)=0) ) and
                   ( (nvl(U.MANIOBRA_APERT_FASE_C,0)+nvl(U.MANIOBRA_CIERRE_FASE_C,0)>nvl(U.MANIOBRA_APERT_FASE_C,0)) or (nvl(U.MANIOBRA_APERT_FASE_C,0)+nvl(U.MANIOBRA_CIERRE_FASE_C,0)=0) )
                 )
             AND    v.est_actual <> 11
             AND    v.alcance = 2
             AND    V.COD_CAUSA = X.COD_CAUSA
             AND    w.subtipo = x.subtipo
             AND    w.subtipo = t.subtipo
             AND    t.GPO_CAUSA = x.gpo_causa
             AND    t.tipo = x.tipo) c
    WHERE    a.instalacion_origen = c.nro_instalacion";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "XMD_INTERRUPCIONES",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta1,
                },
                IdGrupoEjecucion = 1,
            });
            //interucciones negativas
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INTERRUP_DURACION_NEGATIVA");
            consulta1 = @" SELECT NRO_INCIDENCIA, F_REPOSICION, F_ALTA, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, MATRICULA_CT_APOYO, CODIGO_CT
                           FROM TB_DIARIO_CETSA_XMD_INTERRUPCIONES  WHERE  DURACION < 0
                           GROUP BY NRO_INCIDENCIA, F_REPOSICION, F_ALTA, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, MATRICULA_CT_APOYO, CODIGO_CT
                           EXCEPT
                           SELECT NRO_INCIDENCIA, F_REPOSICION, F_ALTA, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, MATRICULA_CT_APOYO, CODIGO_CT
                           FROM TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                ConsultaFinal = consulta1,
                IdGrupoEjecucion = 1,
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });
            //incidencias genericas
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INCIDENCIAS_GENERICAS");
            consulta1 = @"SELECT NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR  
                            FROM
                            (
							SELECT NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR
                            FROM TB_DIARIO_CETSA_XMD_INCIDENCIAS WHERE UPPER(RTRIM(LTRIM(CAUSA_DESC))) = 'GENERICA'
                            GROUP BY NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR
                            UNION ALL
                            SELECT NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR
                            FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES
                            WHERE UPPER(RTRIM(LTRIM(CAUSA_DESC))) = 'GENERICA' 
                            GROUP BY NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR) T
							              EXCEPT 
                            SELECT NRO_INCIDENCIA, PERIODO_SIR,VERSION_SIR 
                            FROM TB_DIARIO_CETSA_XMD_INCIDENCIAS_GENERICAS";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                ConsultaFinal = consulta1,
                IdGrupoEjecucion = 1,
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });
            //Interrucciones mayor 8 horas
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INTERRUP_MAYOR_OCHO_HORAS");
            consulta1 = @"SELECT NRO_INCIDENCIA, DESCRIPCION_CIRCUITO, SECTOR, DESCRIPCION, DURACION, F_REPOSICION, F_ALTA,
                                 INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, USUARIO, OBSERVACION, PERIODO_SIR, MATRICULA_CT_APOYO,
                                 CODIGO_CT,VERSION_SIR
                        FROM TB_DIARIO_CETSA_XMD_INTERRUPCIONES
                        WHERE  (DURACION)/60 > 8 AND  upper(OBSERVACION) not like 'TR %'
                        GROUP BY NRO_INCIDENCIA, DESCRIPCION_CIRCUITO, SECTOR, DESCRIPCION, DURACION, F_REPOSICION, F_ALTA,
                        INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, USUARIO, OBSERVACION, PERIODO_SIR, MATRICULA_CT_APOYO,CODIGO_CT,VERSION_SIR
                        EXCEPT 
                        SELECT NRO_INCIDENCIA, DESCRIPCION_CIRCUITO, SECTOR, DESCRIPCION, DURACION, F_REPOSICION, F_ALTA, INCIDENCIA_MANIO_APER,
                        INCIDENCIA_MANIO_CIERRE, USUARIO, OBSERVACION, PERIODO_SIR, MATRICULA_CT_APOYO, CODIGO_CT,VERSION_SIR
                        FROM TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS 
                        WHERE PERIODO_SIR = @periodo";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                ConsultaFinal = consulta1,
                IdGrupoEjecucion = 1,
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });


            //Interrucciones duplicadas
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XMD_INTERRUP_DUPLICADAS");
            consulta1 = @"select NRO_INCIDENCIA, CODIGO, F_ALTA, F_REPOSICION, DURACION, INC_AGRUPADA, USUARIO, MATRICULA_TRAFO, MATRICULA_CT_APOYO, CODIGO_CT, OBSERVACIONES, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, F_ACTUAL
                          from
                          (
                            select NRO_INCIDENCIA, CODIGO, F_ALTA, F_REPOSICION, DURACION, INC_AGRUPADA, USUARIO, MATRICULA_TRAFO, MATRICULA_CT_APOYO, CODIGO_CT, OBSERVACIONES, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE,[control], f_actual
                            from (
                                select a.nro_incidencia, a.codigo, a.f_alta, a.f_reposicion, a.duracion, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT,
                                       'X' as observaciones, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, 1 as [control], a.f_actual
                                from TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,
                                     (select i.nro_incidencia, i.codigo, i.f_alta, i.f_reposicion
                                      from  (SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                                FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a ,
                                                (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3 group by codigo having count(1) > 1) b
                                                WHERE b.codigo = a.codigo
                                                and a.DURACION > 3                                      
                                            )i,
                                            (
                                                SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                                FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,
                                                      (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                                WHERE b.codigo = a.codigo
                                                and a.DURACION > 3                                      
                                            )f
                                            where i.codigo = f.codigo
                                            and i.idrow <> f.idrow
                                            and i.f_alta < f.f_alta 
                                            and i.f_reposicion > f.f_alta
                                            and i.f_reposicion < f.f_reposicion
                                      ) b
                                where a.nro_incidencia = b.nro_incidencia
                                and   a.codigo = b.codigo
                                and   a.f_alta = b.f_alta
                                and   a.f_reposicion = b.f_reposicion
                                union all
                                select f.nro_incidencia, f.codigo, f.f_alta, f.f_reposicion, f.duracion, f.INC_AGRUPADA, f.USUARIO, f.MATRICULA_TRAFO, f.MATRICULA_CT_APOYO, F.CODIGO_CT,
                                       'la incidencia '+ CONVERT(VARCHAR(100),f.nro_incidencia) + ' esta parcialmente contenida en la incidencia ' + CONVERT(VARCHAR(100),i.nro_incidencia) as observaciones,
                                       f.PERIODO_SIR, f.INCIDENCIA_MANIO_APERT, f.INCIDENCIA_MANIO_CIERRE, 2 as control, f.f_actual
                                from
                                        (
                                            SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.f_actual
                                            FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                            WHERE b.codigo = a.codigo
                                            and a.DURACION > 3                                  
                                        )i,
                                        (
                                            SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.f_actual
                                            FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                            WHERE b.codigo = a.codigo
                                            and a.DURACION > 3                                  
                                        )f
                                where i.codigo = f.codigo
                                and i.idrow <> f.idrow
                                and i.f_alta < f.f_alta 
                                and i.f_reposicion > f.f_alta
                                and i.f_reposicion < f.f_reposicion
                                union all
                                select a.nro_incidencia, a.codigo, a.f_alta, a.f_reposicion, a.duracion, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT,
                                           'X' as observaciones, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, 1 as control, a.f_actual
                                    from TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,
                                         (select i.nro_incidencia, i.codigo, i.f_alta, i.f_reposicion
                                          from  (SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                                    WHERE b.codigo = a.codigo
                                                    and a.DURACION > 3
                                                )i,
                                                (
                                                    SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                                    WHERE b.codigo = a.codigo
                                                    and a.DURACION > 3
                                                )f
                                                where i.codigo = f.codigo
                                                and i.idrow <> f.idrow
                                                and i.f_alta <= f.f_alta
                                                and i.f_reposicion > f.f_alta
                                                and i.f_reposicion >= f.f_reposicion
                                         ) b
                                where a.nro_incidencia = b.nro_incidencia
                                and   a.codigo = b.codigo
                                and   a.f_alta = b.f_alta
                                and   a.f_reposicion = b.f_reposicion
                                union all
                                select f.nro_incidencia, f.codigo, f.f_alta, f.f_reposicion, f.duracion, f.INC_AGRUPADA, f.USUARIO, f.MATRICULA_TRAFO, f.MATRICULA_CT_APOYO, F.CODIGO_CT,
                                       'la incidencia '+CONVERT(VARCHAR(100),f.nro_incidencia)+' esta totalmente contenida en la incidencia '+CONVERT(VARCHAR(100),i.nro_incidencia) as observaciones,
                                       f.PERIODO_SIR, f.INCIDENCIA_MANIO_APERT, f.INCIDENCIA_MANIO_CIERRE, 2 as control, f.f_actual
                                from
                                (
                                    SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.f_actual
                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                    WHERE b.codigo = a.codigo
                                    and a.DURACION > 3                          
                                )i,
                                (
                                    SELECT a.ID_TABLA as idrow, a.NRO_INCIDENCIA, a.CODIGO, a.F_ALTA, a.F_REPOSICION, a.DURACION, a.INC_AGRUPADA, a.USUARIO, a.MATRICULA_TRAFO, a.MATRICULA_CT_APOYO, CODIGO_CT, a.PERIODO_SIR, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.f_actual
                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUPCIONES a,  (select codigo, count(1) n from TB_DIARIO_CETSA_XMD_INTERRUPCIONES where DURACION > 3   group by codigo having count(1) > 1) b
                                    WHERE b.codigo = a.codigo
                                    and a.DURACION > 3                          
                                )f
                                where i.codigo = f.codigo
                                and i.idrow <> f.idrow
                                and i.f_alta <= f.f_alta
                                and i.f_reposicion > f.f_alta
                                and i.f_reposicion >= f.f_reposicion
                            ) b
                            group by NRO_INCIDENCIA, CODIGO, F_ALTA, F_REPOSICION, DURACION, INC_AGRUPADA, USUARIO, MATRICULA_TRAFO, MATRICULA_CT_APOYO, CODIGO_CT, OBSERVACIONES, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE,[control], f_actual
                          )c                  
                          EXCEPT 
                          SELECT NRO_INCIDENCIA, CODIGO, F_ALTA, F_REPOSICION, DURACION, INC_AGRUPADA, USUARIO, MATRICULA_TRAFO, MATRICULA_CT_APOYO, CODIGO_CT, OBSERVACIONES, PERIODO_SIR, INCIDENCIA_MANIO_APERT, INCIDENCIA_MANIO_CIERRE, F_ACTUAL
                          FROM TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS
                          WHERE PERIODO_SIR = @periodo
                          order by codigo,nro_incidencia asc";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 8,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                ConsultaFinal = consulta1,
                IdGrupoEjecucion = 1,
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 9,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });
            //Avisos pendientes
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XM_AVISOS_PENDIENTES");
            var consulta4 = @"
                select sum.nic,
                a.nis_rad,
                a.PRIORIDAD,
                a.nombre,
                a.ape1||' '||a.ape2 as apellidos,
                a.usuario,
                a.nro_aviso aviso,
                UPPER(c.nom_usuario) NOMBRE_USUARIO,
                case
                  when a.CLASE_AVISO = 0 and a.nro_instalacion =  '88888888' then 'SIN ALIMENTACION'
                  when a.CLASE_AVISO = 0 and a.nro_instalacion <> '88888888' then 'CON ALIMENTACION'
                  else upper(e.descripcion)
                end Clase_aviso,
                f.descripcion tipo_aviso,
                upper(b.descripcion) as estado_aviso,
                decode(ind_cli_imp, 0,'NO',1,'SI','N/A') as Cliente_VIP,
                a.nro_llamadas,
                a.nro_ot,
                a.nro_incidencia,
                a.f_alta as fecha_ingreso_aviso,
                a.fecha_res as fecha_resuelto_AVISO,
                trunc((sysdate - a.f_alta)*24,2) tiempo_total_horas,
                d.nom_centro as zona,
                A.NOM_PROV DEPTO,
                A.NOM_DEPTO MUNICIPIO,
                a.nom_munic CORREGIMIENTO,
                A.NOM_LOC LOCALIDAD,
                a.esquina  direccion, 
                replace(replace(a.obs_telegestiones,CHR(13),''),CHR(10),'') as OBSERVACION_TELEGESTIONES,
                to_date(TO_CHAR(a.f_alta, 'DD/MM/YYYY HH24:MI:SS'),'DD/MM/YYYY HH24:MI:SS') fecha_hhmmss_ingreso_aviso,
                to_date(TO_CHAR(a.fecha_res, 'DD/MM/YYYY HH24:MI:SS'),'DD/MM/YYYY HH24:MI:SS') fecha_hhmmss_resuelto_aviso,
                to_char(ind_cli_imp) Cod_Cliente_VIP
              from
                gi_avisos a,
                sgd_valor b,
                gi_usuario_sgi c,
                sgd_centro d,
                sgd_valor e,
                GI_T_TIP_AVISO f,
                sumcon sum
               where A.EST_AVISO = 1
                and a.nro_centro in(4000, 5000)
                and sum.nis_rad (+) = a.nis_rad
                and sum.tip_serv (+) like 'SV1%'
                and a.usuario = c.usuario  (+)
                and b.codif = 'E_AV'
                and e.codif = 'C_AV'
                and b.codigo = A.EST_AVISO
                and a.tip_Aviso = f.tip_aviso
                and e.codigo = a.clase_aviso
                and a.nro_cmd = D.NRO_CENTRO  (+)
                AND a.F_ALTA <= to_date(@periodo||' 23:59:59','yyyymmdd hh24:mi:ss')
                AND a.F_ALTA >= to_date('20190601 00:00:00','yyyymmdd hh24:mi:ss')";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 10,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "AVISOS_PENDIENTES",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4                   
                },
                IdGrupoEjecucion = 1,
                IdCategoria = 1,
                ConsultaFinal = @"
                    SELECT  NIC,NIS_RAD,PRIORIDAD,NOMBRE,APELLIDOS,USUARIO,AVISO,NOMBRE_USUARIO,
                            CLASE_AVISO,TIPO_AVISO,ESTADO_AVISO,CLIENTE_VIP,NRO_LLAMADAS,
                            NRO_OT,NRO_INCIDENCIA,FECHA_INGRESO_AVISO,FECHA_RESUELTO_AVISO,
                            TIEMPO_TOTAL_HORAS,ZONA,DEPTO,MUNICIPIO,CORREGIMIENTO,LOCALIDAD,DIRECCION, 
                            OBSERVACION_TELEGESTIONES,FECHA_HHMMSS_INGRESO_AVISO,FECHA_HHMMSS_RESUELTO_AVISO,
                            COD_CLIENTE_VIP
                    FROM Temp_Combinacion_XM_AVISOS_PENDIENTES
                    EXCEPT
                    SELECT  NIC,NIS_RAD,PRIORIDAD,NOMBRE,APELLIDOS,USUARIO,AVISO,NOMBRE_USUARIO,
                            CLASE_AVISO,TIPO_AVISO,ESTADO_AVISO,CLIENTE_VIP,NRO_LLAMADAS,
                            NRO_OT,NRO_INCIDENCIA,FECHA_INGRESO_AVISO,FECHA_RESUELTO_AVISO,
                            TIEMPO_TOTAL_HORAS,ZONA,DEPTO,MUNICIPIO,CORREGIMIENTO,LOCALIDAD,DIRECCION, 
                            OBSERVACION_TELEGESTIONES,FECHA_HHMMSS_INGRESO_AVISO,FECHA_HHMMSS_RESUELTO_AVISO,
                            COD_CLIENTE_VIP
                    FROM TB_DIARIO_CETSA_XM_AVISOS_PENDIENTES 
                    WHERE PERIODO_SIR = @periodo"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 11,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "XM_TRAFOS_ABIERTOS");

            consulta4 = @"SELECT distinct c.nro_incidencia,
                                a.cod_circuito cod_circuito,
                                c.f_alta,
                                a.sspdid AS sspdid
                            FROM (SELECT 
                                    j.instalacion_origen,
                                    j.nombre,
                                    j.cod_circuito,
                                    j.sspdid,
                                    j.nro_Centro
                                 FROM (SELECT codigo, descripcion
                                         FROM   bdiv10_maestro
                                         WHERE  tip_tabla = 'PROP') l,
                                        (SELECT SUBSTR(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10,':', 1)+1) as instalacion_origen,
                                            C.nombre,
                                            c.codigo as cod_circuito,
                                            a.sspdid,
                                            a.nro_Centro,
                                            A.PROPIEDAD
                                          FROM
                                            bdiv10_sgd_salmt c,
                                            bdiv10_sgd_ct b,
                                            bdiv10_sgd_trafo_mb a
                                          WHERE B.CODIGO = SUBSTR(A.INSTALACION_ORIGEN_V10,INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1)
                                            AND C.CODIGO = SUBSTR(B.INSTALACION_ORIGEN_V10,INSTR(B.INSTALACION_ORIGEN_V10, ':', 1)+1)
                                            AND c.onis_stat = '0'
                                            AND c.onis_ver not like '%.%'
                                            AND b.onis_stat = '0'
                                            AND b.onis_ver not like '%.%'
                                            AND a.onis_stat = '0'
                                            AND a.onis_ver not like '%.%') j
                                 WHERE  j.propiedad = l.codigo(+)) a,
                                (SELECT u.nro_incidencia,
                                    u.nro_instalacion,
                                    u.f_alta,
                                    v.cod_causa
                                 FROM sgd_interrupcion u,
                                    sgd_incidencia v
                                 WHERE  v.nro_incidencia = u.nro_incidencia
                                  AND    u.f_reposicion is null
                                  and    u.f_alta<to_date( to_char( to_date(@periodo,'yyyymmdd')+1,'yyyymmdd'  )||' 00:00:00','yyyymmdd hh24:mi:ss')
                                  AND    v.est_actual <> 11
                                  AND    v.alcance = 2) c,
                                (
                                   select S.DESCRIPCION as TIPO_CAUSA, trim(SC.DESCRIPCION) as GRUPO_CAUSA, C.DESCRIPCION as CAUSA, C.COD_CAUSA
                                   from gi_causa c, GI_SUBTIPO_CAUSA sc, gi_subtipos s
                                   where (S.TIPO=SC.TIPO and S.SUBTIPO=SC.SUBTIPO)
                                        and (SC.TIPO=C.TIPO and SC.SUBTIPO=C.SUBTIPO)
                                        and (SC.GPO_CAUSA=C.GPO_CAUSA)
                                ) caus
                            WHERE    a.instalacion_origen = c.nro_instalacion
                              and c.cod_causa=caus.cod_causa(+)";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 12,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "TRAFOS_ABIERTOS",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                IdGrupoEjecucion = 1,
                IdCategoria = 1,
                ConsultaFinal = @"
                    SELECT  NRO_INCIDENCIA,COD_CIRCUITO,F_ALTA,TRANSFORMADOR
                    FROM Temp_Combinacion_XM_TRAFOS_ABIERTOS
                    EXCEPT
                    SELECT  NRO_INCIDENCIA,COD_CIRCUITO,F_ALTA,TRANSFORMADOR
                    FROM TB_DIARIO_CETSA_XM_TRAFOS_ABIERTOS
                    WHERE PERIODO_SIR = @periodo"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 13,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 1
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 14,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Validar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = 0,
                IdGrupoEjecucion = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 15,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Confirmar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = 0,
                IdGrupoEjecucion = 3
            });

            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TODAS_INTERRUCCIONES");
            MODArchivo archivo = archivos.FirstOrDefault(x => x.Nombre == "INTERRUPCIONES");
            //OPEN ASOCIA
            var QUERY_TRSF_S6_EPSA = @"SELECT 
                        ACCION,
                        NRO_INCIDENCIA,
                        F_ALTA,
                        F_REPOSICION,
                        NOMBRE,
                        CT_SALMT,
                        F_ALTA_NVA,
                        F_REPOSICION_NVA,
                        F_ACTUAL,
                        NRO_INCIDENCIA_NVA,
                        MANIOBRA_APERT,
                        MANIOBRA_CIERRE,
                        MANIOBRA_APERT_NVA,
                        MANIOBRA_CIERRE_NVA  
                  FROM 
                  (       
                     SELECT    (select marca b from ListaAcciones b where b.id = a.IDCAUSA_SIR) as accion,
                              a.NRO_INCIDENCIA,
                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,                              
							  a.MATRICULA_CT_APOYO NOMBRE,
                              a.CODIGO_CT CT_SALMT,
                              --CASE WHEN a.F_ALTA_ANT IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),a.F_ALTA_ANT, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA_ANT, 8) F_ALTA_NVA,
                			  CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,                              
							  CONVERT(VARCHAR(10),a.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),a.F_ACTUAL, 8) F_ACTUAL,
                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                          FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
						  LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN
                          --WHERE  a.VERSION_SIR = V_VERSION_SIR
                          WHERE A.IDCAUSA_SIR = '8' AND PERIODO_SIR = @periodo
						  
                UNION
                 SELECT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, D.matricula_ct_apoyo AS NOMBRE, d.codigo_ct as CT_SALMT, C.F_ALTA_NVA, C.F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, C.MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, C.MANIOBRA_CIERRE_NVA
                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    WHERE  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR = '3' 
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                      --AND  a.VERSION_SIR = V_VERSION_SIR                      
                                    group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA, 8)
                              AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8)
                			  AND PERIODO_SIR = @periodo
                	UNION
                    SELECT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8), D.matricula_ct_apoyo AS NOMBRE, d.codigo_ct as CT_SALMT, C.F_ALTA_NVA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8) F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, 
                	1012351005 MANIOBRA_CIERRE, 
                	 C.MANIOBRA_APERT_NVA, 
                	1012351005 MANIOBRA_CIERRE_NVA
                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    WHERE  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR = '1'
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                      --AND  a.VERSION_SIR = V_VERSION_SIR
                                   group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA,8)
                			  AND PERIODO_SIR = @periodo

                UNION
                    SELECT  C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, D.matricula_ct_apoyo AS NOMBRE, d.codigo_ct as CT_SALMT, c.F_ALTA_NVA F_ALTA_NVA, C.F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, 
                			100000 MANIOBRA_APERT, C.MANIOBRA_CIERRE, 100000 MANIOBRA_APERT_NVA, C.MANIOBRA_CIERRE_NVA
                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    WHERE  A.IDCAUSA_SIR = B.ID
                                      AND  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR = '2'
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                    group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8)
                			  AND PERIODO_SIR = @periodo

                		UNION
                         SELECT   (select marca b from ListaAcciones b where B.ID = A.IDCAUSA_SIR) as accion,
                                  a.NRO_INCIDENCIA,
                                  CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                  CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                  a.MATRICULA_CT_APOYO NOMBRE,
                                  a.CODIGO_CT CT_SALMT,
                                  CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                  CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                  CONVERT(VARCHAR(10),a.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),a.F_ACTUAL, 8) F_ACTUAL,
                                  a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                  a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                  a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                  a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                  a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                              FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a
							  LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    
                              --WHERE  a.VERSION_SIR = V_VERSION_SIR
                              WHERE  A.IDCAUSA_SIR = '8' AND PERIODO_SIR = @periodo
                UNION                
                        SELECT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, D.matricula_ct_apoyo AS NOMBRE, d.codigo as CT_SALMT, C.F_ALTA_NVA, C.F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, C.MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, C.MANIOBRA_CIERRE_NVA
                                    FROM  (
                                            SELECT    b.marca as accion, 
                                                      a.NRO_INCIDENCIA,
                                                      CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                                      CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                                      CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                                      CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                                      a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                            FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a
											INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
											LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'                           
                                            WHERE  A.IDCAUSA_SIR = B.ID
                                              AND  B.marca IS NOT NULL
                                              AND  A.IDCAUSA_SIR in ('3') 
                                              AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                              --AND  a.VERSION_SIR = V_VERSION_SIR

                                            group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                          ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                                    WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                                      AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA,8)
                                      AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8)
                					  AND PERIODO_SIR = @periodo

                			UNION
                            SELECT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, d.matricula_ct_apoyo AS NOMBRE, codigo_ct as CT_SALMT, C.F_ALTA_NVA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8) F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, 
                			1012351005 MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, 
                			1012351005 MANIOBRA_CIERRE_NVA
                                    FROM  (
                                            SELECT    b.marca as accion, 
                                                      a.NRO_INCIDENCIA,
                                                      CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                                      CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                                      CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                                      CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                                                      a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                            FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a 
											INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
											LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'                                    
                                            WHERE  A.IDCAUSA_SIR = B.ID
                                              AND  B.marca IS NOT NULL
                                              AND  A.IDCAUSA_SIR = '1'
                                              AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                              --AND  a.VERSION_SIR = V_VERSION_SIR
                                            group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                          ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                                    WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                                      AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA, 8)
                					  AND PERIODO_SIR = @periodo

                	UNION
                            SELECT DISTINCT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, d.matricula_ct_apoyo AS NOMBRE, codigo_ct as CT_SALMT, C.F_ALTA_NVA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8) F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, 
                			1012351005 MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, 
                			1012351005 MANIOBRA_CIERRE_NVA
                            FROM  (
                                    SELECT    b.marca as accion, 
                                                      a.NRO_INCIDENCIA,
                                                      CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                                      CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                                      CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                                      CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                                      a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                                      a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                                                      a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                            FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a 
											INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
											LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                            WHERE  A.IDCAUSA_SIR = B.ID
                                              AND  B.marca IS NOT NULL
                                              AND  A.IDCAUSA_SIR = '2'
                                              AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                              --AND  a.VERSION_SIR = V_VERSION_SIR
                                            group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,
                                                     a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APERT, a.INCIDENCIA_MANIO_CIERRE
                                          ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                			AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8)
                			AND PERIODO_SIR = @periodo

                UNION   
                         SELECT   (select marca b from ListaAcciones b where B.ID = A.IDCAUSA_SIR) as accion,
                                  a.NRO_INCIDENCIA,
                                  CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                  CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                  a.MATRICULA_CT_APOYO NOMBRE,
                                  a.CODIGO_CT CT_SALMT,
                                  CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                  CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                                  CONVERT(VARCHAR(10),a.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),a.F_ACTUAL, 8) F_ACTUAL,
                                  a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                  a.INCIDENCIA_MANIO_APER as MANIOBRA_APERT,
                                  a.INCIDENCIA_MANIO_CIERRE as MANIOBRA_CIERRE,
                                  a.INCIDENCIA_MANIO_APER as MANIOBRA_APERT_NVA,
                                  a.INCIDENCIA_MANIO_CIERRE as MANIOBRA_CIERRE_NVA
                              FROM  [TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS] a
								INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
								LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                              --WHERE  a.VERSION_SIR = V_VERSION_SIR
                              WHERE A.IDCAUSA_SIR = '8'
                			  AND PERIODO_SIR = @periodo
                UNION 
                    SELECT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, D.matricula_ct_apoyo AS NOMBRE, d.cod_circuito as CT_SALMT, C.F_ALTA_NVA, C.F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL, 8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, C.MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, C.MANIOBRA_CIERRE_NVA
                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    WHERE  A.IDCAUSA_SIR = B.ID
                                      AND  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR in('3') 
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                      --AND  a.VERSION_SIR = V_VERSION_SIR
                					  AND PERIODO_SIR = @periodo

                                    group by b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION, t0.F_ALTA,t0.F_REPOSICION,
                                             a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APER, a.INCIDENCIA_MANIO_CIERRE, a.INCIDENCIA_MANIO_APER, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA,8)
                              AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8)
                			  AND D.PERIODO_SIR = @periodo

                UNION
                      SELECT DISTINCT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, d.matricula_ct_apoyo AS NOMBRE, codigo_ct as CT_SALMT, C.F_ALTA_NVA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8) F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, 
                			1012351005 MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, 
                			1012351005 MANIOBRA_CIERRE_NVA

                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                                    WHERE  A.IDCAUSA_SIR = B.ID
                                      AND  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR = '1'
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                					  AND  PERIODO_SIR = @periodo
                                      --AND  a.VERSION_SIR = V_VERSION_SIR                      
                                    group by  b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APER, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_ALTA = CONVERT(VARCHAR(10),D.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),D.F_ALTA,8)
                			  AND PERIODO_SIR = @periodo

                UNION
                    SELECT DISTINCT C.accion, C.NRO_INCIDENCIA, C.F_ALTA, C.F_REPOSICION, d.matricula_ct_apoyo AS NOMBRE, codigo_ct as CT_SALMT, C.F_ALTA_NVA, CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION, 8) F_REPOSICION_NVA, CONVERT(VARCHAR(10),D.F_ACTUAL, 103)+' '+CONVERT(VARCHAR(10),D.F_ACTUAL,8) F_ACTUAL, C.NRO_INCIDENCIA_NVA, C.MANIOBRA_APERT, 
                			1012351005 MANIOBRA_CIERRE, C.MANIOBRA_APERT_NVA, 
                			1012351005 MANIOBRA_CIERRE_NVA            
                            FROM  (
                                    SELECT    b.marca as accion, 
                                              a.NRO_INCIDENCIA,
                                              CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                                              CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8)END F_REPOSICION,
                                              CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                                              CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA, 
                                              a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                                              a.INCIDENCIA_MANIO_APER MANIOBRA_APERT_NVA,
                                              a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                                    FROM   TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS a
									INNER JOIN ListaAcciones b ON A.IDCAUSA_SIR = B.ID
									LEFT JOIN [TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD] t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'                                    
                                    WHERE  A.IDCAUSA_SIR = B.ID
                                      AND  B.marca IS NOT NULL
                                      AND  A.IDCAUSA_SIR = '2'
                                      AND  A.CONFIRMACION_SIR IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                                      --AND  a.VERSION_SIR = V_VERSION_SIR
                					  AND PERIODO_SIR = @periodo    
                                    group by  b.marca, a.NRO_INCIDENCIA, a.F_ALTA,a.F_REPOSICION,t0.F_ALTA,t0.F_REPOSICION,a.NRO_INCIDENCIA, a.INCIDENCIA_MANIO_APER, a.INCIDENCIA_MANIO_CIERRE
                                  ) C,[TB_DIARIO_CETSA_XMD_INTERRUPCIONES] D
                            WHERE C.NRO_INCIDENCIA = D.NRO_INCIDENCIA
                              AND C.F_REPOSICION = CONVERT(VARCHAR(10),D.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),D.F_REPOSICION,8)
                			  AND PERIODO_SIR = @periodo
                ) a";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 16,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "INTERRUPCIONES_TODAS",
                    Sid = "SIR2",
                    //NombreTabla = "TRSF_EPSA",//colocar control no mas 20 caracteres
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_TRSF_S6_EPSA
                },
                Agrupador = "Extraccion",
                IdGrupoEjecucion = 4,
                IdCategoria = 1
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 17,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 4,
                IdArchivo = archivo.IdArchivo,
                Agrupador = "Extraccion",
                IdCategoria = 1
            });
            //TRSF_S6
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TODAS_MANIOBRAS");
            archivo = archivos.FirstOrDefault(x => x.Nombre == "MANIOBRAS");
            var QUERY_TRSF_S6_CETSA = @"                                
                SELECT DISTINCT
                      ACCION,
                      NRO_INCIDENCIA,
                      COD_MANIOBRA,
                      F_MANIOBRA,
                      NRO_INCIDENCIA_NVA,
                      F_MANIOBRA_NVA
                     FROM
                    (
                      SELECT
                          D.MARCA  ACCION,
                          NRO_INCIDENCIA      NRO_INCIDENCIA,
                          MANIOBRA_APERT      COD_MANIOBRA,
                          F_ALTA              F_MANIOBRA,
                          NRO_INCIDENCIA_NVA  NRO_INCIDENCIA_NVA,
                          F_ALTA_NVA          F_MANIOBRA_NVA
                    FROM 
                    (       
                      SELECT 
                            a.IDCAUSA_SIR ID_OPCION,
                			CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) AS F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                        WHERE PERIODO_SIR = @periodo
                        UNION ALL    
                        SELECT 
                            a.IDCAUSA_SIR ID_OPCION,
                			a.CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
						WHERE PERIODO_SIR = @periodo
                        UNION ALL
                          SELECT 
                            a.IDCAUSA_SIR ID_OPCION, CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT  CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APER MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APER MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                        WHERE PERIODO_SIR = @periodo
                      ) B,
                      ListaAcciones D
                      WHERE B.ID_OPCION = D.ID
                      AND D.MARCA = 'A'
                      AND F_ALTA != F_ALTA_NVA
                      AND B.ID_ESTADO IN ('2','4')
                      UNION ALL
                       SELECT 
                          D.MARCA ACCION,
                          NRO_INCIDENCIA      NRO_INCIDENCIA,
                          MANIOBRA_CIERRE     COD_MANIOBRA,
                          F_REPOSICION        F_MANIOBRA,
                          NRO_INCIDENCIA_NVA  NRO_INCIDENCIA_NVA,
                          F_REPOSICION_NVA    F_MANIOBRA_NVA
                    FROM 
                    (                             
                      SELECT 
                            a.IDCAUSA_SIR ID_OPCION, CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION_NVA,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
						
                        WHERE PERIODO_SIR = @periodo
                        UNION ALL                            
                        SELECT 
                            a.IDCAUSA_SIR ID_OPCION, CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APERT MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   [TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS] a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                        WHERE PERIODO_SIR = @periodo
                        UNION ALL                          
                          SELECT 
                            a.IDCAUSA_SIR ID_OPCION, CONFIRMACION_SIR ID_ESTADO,
                            a.NRO_INCIDENCIA,
                            CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) F_ALTA_NVA,
                            CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) F_REPOSICION,
                            a.MATRICULA_CT_APOYO NOMBRE,
                            a.CODIGO_CT  CT_SALMT,
                            CASE WHEN t0.F_ALTA IS NULL THEN CONVERT(VARCHAR(10),a.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA, 8) ELSE CONVERT(VARCHAR(10),t0.F_ALTA, 103)+' '+CONVERT(VARCHAR(10),t0.F_ALTA, 8) END F_ALTA,
                            CASE WHEN t0.F_REPOSICION IS NULL THEN CONVERT(VARCHAR(10),a.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),a.F_REPOSICION, 8) ELSE CONVERT(VARCHAR(10),t0.F_REPOSICION, 103)+' '+CONVERT(VARCHAR(10),t0.F_REPOSICION, 8) END F_REPOSICION,
                            CONVERT(VARCHAR(10),a.F_ALTA+1, 103)+' '+CONVERT(VARCHAR(10),a.F_ALTA+1, 8) F_ACTUAL,
                            a.NRO_INCIDENCIA NRO_INCIDENCIA_NVA,
                            a.INCIDENCIA_MANIO_APER MANIOBRA_APERT,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE,
                            a.INCIDENCIA_MANIO_APER MANIOBRA_APERT_NVA,
                            a.INCIDENCIA_MANIO_CIERRE MANIOBRA_CIERRE_NVA
                        FROM   TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS a
						LEFT JOIN TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS_AUD t0 ON a.ID_TABLA = t0.ID_ORIGEN AND t0.MARCA = 'O'
                        WHERE PERIODO_SIR = @periodo
                      ) B,
                      ListaAcciones D
                      WHERE B.ID_OPCION = D.ID
                      AND D.MARCA = 'A'
                      AND F_REPOSICION != F_REPOSICION_NVA
                      AND B.ID_ESTADO IN ('2','4') -- PARA PODER VISUALIZAR DEBE ESTAR EN ESTADO APROBADO O EXPORTADO
                    ) A
                	ORDER BY NRO_INCIDENCIA";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 18,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Sid = "SIR2",
                    Nombre = "XMD_MANIOBRAS",
                    //NombreTabla = "XMD_MANIOBRAS",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_TRSF_S6_CETSA
                },
                Agrupador = "Extraccion",
                IdGrupoEjecucion = 4,
                IdCategoria = 1
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 19,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 4,
                IdArchivo = archivo.IdArchivo,
                Agrupador = "Extraccion",
                IdCategoria = 1
            });

            //Archivo CETSA
            //Extraccion proceso diario
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "EXTR_XM_DIARIO_NF");
            
            var x = @"select t.*
                        from (
                            select distinct
                              to_char(a.nro_incidencia) as codigo_evento,          
                              case when trunc(b.f_alta) < to_date(@periodo,'yyyymmdd') then null else to_char(b.f_alta, 'dd/mm/yyyy hh24:mi:ss') end as fecha_inicial,          
                              case when trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') then null else to_char(b.f_reposicion, 'dd/mm/yyyy hh24:mi:ss') end as fecha_final,
                              trafo.codigo as codigo_elemento,
                              '1' as tipo_elemento,
                              case when trunc(b.f_alta) < to_date(@periodo,'yyyymmdd') and trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') then 'S'
                                   when trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') or b.f_reposicion is null then 'S'
                                   else 'N'
                              end EventoConinua,
                              'EPSA' as empresa,
                              nvl(b.str_a,0)+nvl(b.str_b,0)+nvl(b.str_c,0) STR,
                              null CABECERA_MT,
                              c.COD_REPORTE_TRIMESTRAL_LAC cod_causa,
                              decode(sb.ind_zni,'S','1','0') EVENTO_EXCL_ZNI,
                              decode(nvl(ct.potencia_instalada_ag_gd,0),0,0,1) AFECTA_CONEXION_GEN,
                              CASE WHEN trafo.tipo_servicio IN ('1','2') THEN '1'
                                   WHEN trafo.propiedad IN ('5','6') AND trafo.cant_clientes >= 2 THEN '1'
                                   ELSE '0'
                              END userPub,
                              trafo.cod_trafo
                            from (select sspdid as codigo,
                                    substr(instalacion_origen_v10, instr(instalacion_origen_v10, ':',1)+1) as instalacion_origen,
                                    instalacion_origen_v10,
                                    tip_con_ten tipo_servicio,
                                    propietario_trafo propiedad,
                                    cant_clientes,
                                    codigo cod_trafo
                                  from bdiv10_sgd_trafo_mb c
                                  where onis_stat = '0' and onis_ver not like '%.%'
                                    and nro_centro = 4000
                                    and NVL(SSPDID,'0') <> '0'
                                    and CANT_CLIENTES>0
                                    and sspdid not in( select sspdid from SIR_TRAFOS_BAJA )                
                                 ) trafo,
                                 trsf_f4_5_paridad_causas_nf c,
                                 sgd_incidencia a, sgd_interrupcion b,
                                 (select codigo, tipo_ct, longitud, latitud, altitud, instalacion_origen_v10, potencia_instalada_ag_gd from bdiv10_sgd_ct where onis_stat = '0' and onis_ver NOT LIKE '%.%') ct,
                                 (select codigo, nombre, instalacion_origen_v10 from bdiv10_sgd_salmt where onis_stat = '0' and onis_ver not like '%.%') mt,
                                 /*bdiv10_sgd_subestac sb*/
                                 (select codigo, ind_zni from bdiv10_sgd_subestac where onis_stat = '0' and onis_ver not like '%.%') sb
                            where a.nro_incidencia = b.nro_incidencia
                                and  a.alcance = 2
                                and b.nro_instalacion = trafo.instalacion_origen
                                and a.est_actual <> 11
                                and a.cod_causa != 999
                                and (b.f_reposicion >= to_date(@periodo||' 00:00:00', 'yyyymmdd hh24:mi:ss') or b.f_reposicion is null)            
                                and b.f_alta< to_date(@periodo||' 00:00:00','yyyymmdd hh24:mi:ss')+1
                                AND NOT EXISTS (SELECT 1 FROM SIR_PARIDAD_TRAFO SPT
                                                WHERE to_char(SPT.CODIGO_CT) = trafo.codigo
                                                AND   SPT.ACCION ='Excluir')
                                and SUBSTR(trafo.INSTALACION_ORIGEN_V10, INSTR(trafo.INSTALACION_ORIGEN_V10, ':', 1)+1) = ct.CODIGO(+)
                                and SUBSTR(ct.INSTALACION_ORIGEN_V10, INSTR(ct.INSTALACION_ORIGEN_V10, ':', 1)+1) = mt.CODIGO(+)
                                and SUBSTR(mt.INSTALACION_ORIGEN_V10, INSTR(mt.INSTALACION_ORIGEN_V10, ':', 1)+1) = sb.CODIGO(+)
                                and a.cod_causa = c.id(+)) t
                        where EXISTS (select distinct ci.CODIGO_TRAFO_MB from aud_sumcon s , (select CODIGO_TRAFO_MB, nis_rad, max(f_actual) from gi_cliente_instalacion  group by CODIGO_TRAFO_MB, nis_rad) ci
                              where s.nis_rad= ci.nis_rad
                              and s.tip_suministro != 'SU035'
                              AND (s.tip_serv='SV100' OR s.tip_serv='SV120')
                              and s.nic>0
                              AND t.cod_trafo = ci.CODIGO_TRAFO_MB)
                    UNION ALL
                    select t.*
                        from (
                            select distinct
                              to_char(a.nro_incidencia) as codigo_evento,
                              case when trunc(b.f_alta) < to_date(@periodo,'yyyymmdd') then null else to_char(b.f_alta, 'dd/mm/yyyy hh24:mi:ss') end as fecha_inicial,          
                              case when trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') then null else to_char(b.f_reposicion, 'dd/mm/yyyy hh24:mi:ss') end as fecha_final,
                              trafo.codigo as codigo_elemento,
                              '1' as tipo_elemento,          
                              case when trunc(b.f_alta) < to_date(@periodo,'yyyymmdd') and trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') then 'S'
                                   when trunc(b.f_reposicion) > to_date(@periodo, 'yyyymmdd') or b.f_reposicion is null then 'S'
                                   else 'N'
                              end EventoConinua,
                              'CETSA' as empresa,
                              nvl(b.str_a,0)+nvl(b.str_b,0)+nvl(b.str_c,0) STR,
                              null CABECERA_MT,
                              c.COD_REPORTE_TRIMESTRAL_LAC cod_causa,
                              decode(sb.ind_zni,'S','1','0') EVENTO_EXCL_ZNI,
                              decode(nvl(ct.potencia_instalada_ag_gd,0),0,0,1) AFECTA_CONEXION_GEN,
                              CASE WHEN trafo.tipo_servicio IN ('1','2') THEN '1'
                                   WHEN trafo.propiedad IN ('5','6') AND trafo.cant_clientes >= 2 THEN '1'
                                   ELSE '0'
                              END userPub,
                              trafo.cod_trafo
                            from (select sspdid as codigo,
                                    substr(instalacion_origen_v10, instr(instalacion_origen_v10, ':',1)+1) as instalacion_origen,
                                    instalacion_origen_v10,
                                    tip_con_ten tipo_servicio,
                                    propietario_trafo propiedad,
                                    cant_clientes,
                                    codigo cod_trafo
                                  from bdiv10_sgd_trafo_mb c
                                  where onis_stat = '0' and onis_ver not like '%.%'
                                    and nro_centro = 5000
                                    and NVL(SSPDID,'0') <> '0'
                                    and CANT_CLIENTES>0
                                    and sspdid not in( select sspdid from SIR_TRAFOS_BAJA )
                                 ) trafo,
                                 trsf_f4_5_paridad_causas_nf c,
                                 sgd_incidencia a, sgd_interrupcion b,
                                 (select codigo, tipo_ct, longitud, latitud, altitud, instalacion_origen_v10, potencia_instalada_ag_gd from bdiv10_sgd_ct where onis_stat = '0' and onis_ver NOT LIKE '%.%') ct,
                                 (select codigo, nombre, instalacion_origen_v10 from bdiv10_sgd_salmt where onis_stat = '0' and onis_ver not like '%.%') mt,
                                 /*bdiv10_sgd_subestac sb*/
                                 (select codigo, ind_zni from bdiv10_sgd_subestac where onis_stat = '0' and onis_ver not like '%.%') sb
                            where a.nro_incidencia = b.nro_incidencia
                                and  a.alcance = 2
                                and b.nro_instalacion = trafo.instalacion_origen
                                and a.est_actual <> 11
                                and a.cod_causa != 999
                                and (b.f_reposicion >= to_date(@periodo||' 00:00:00', 'yyyymmdd hh24:mi:ss') or b.f_reposicion is null)            
                                and b.f_alta< to_date(@periodo||' 00:00:00','yyyymmdd hh24:mi:ss')+1
                                AND NOT EXISTS (SELECT 1 FROM SIR_PARIDAD_TRAFO SPT
                                                WHERE to_char(SPT.CODIGO_CT) = trafo.codigo
                                                AND   SPT.ACCION ='Excluir')
                                and SUBSTR(trafo.INSTALACION_ORIGEN_V10, INSTR(trafo.INSTALACION_ORIGEN_V10, ':', 1)+1) = ct.CODIGO(+)
                                and SUBSTR(ct.INSTALACION_ORIGEN_V10, INSTR(ct.INSTALACION_ORIGEN_V10, ':', 1)+1) = mt.CODIGO(+)
                                and SUBSTR(mt.INSTALACION_ORIGEN_V10, INSTR(mt.INSTALACION_ORIGEN_V10, ':', 1)+1) = sb.CODIGO(+)
                                and a.cod_causa = c.id(+)) t
                        where EXISTS ( select distinct ci.CODIGO_TRAFO_MB from aud_sumcon s , (select CODIGO_TRAFO_MB, nis_rad, max(f_actual) from gi_cliente_instalacion  group by CODIGO_TRAFO_MB, nis_rad) ci
                              where s.nis_rad= ci.nis_rad
                              and s.tip_suministro != 'SU035'
                              AND (s.tip_serv='SV100' OR s.tip_serv='SV120')
                              and s.nic>0
                              AND t.cod_trafo = ci.CODIGO_TRAFO_MB)";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 20,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_XM_DIARIO_NF",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = x,
                },
                Agrupador = "Extraccion",
                IdGrupoEjecucion = 5,
                IdCategoria = 1
            });

            x = @"select y2.codigo, y2.sspdid,TRUNC(sysdate) FECHA
                    from 
                      (  select a.codigo_trafo_mb
                         from  ( select gi.codigo_trafo_mb, count(1) as conteo
                                 from (select nis_rad, est_sum from aud_sumcon where tip_serv = 'SV100' or tip_serv = 'SV120') sc, 
                                      gi_cliente_instalacion gi
                                 where sc.nis_rad = gi.nis_rad     
                                 group by gi.codigo_trafo_mb
                               ) a,
                               (
                                 select gi.codigo_trafo_mb, sc.est_sum, count(1) as conteo
                                 from (SELECT Y.NIS_RAD, Y.EST_SUM
                                                FROM /*AUD_RECIBOS R,*/AUD_SUMCON Y
                                              WHERE (Y.TIP_SERV = 'SV100' or Y.TIP_SERV ='SV120')
                                                AND Y.EST_SUM = 'EC000' 
                                                --AND R.NIS_RAD   = Y.NIS_RAD
                                              --GROUP BY Y.NIS_RAD, Y.EST_SUM
                                      ) sc, 
                                      gi_cliente_instalacion gi
                                 where sc.nis_rad = gi.nis_rad     
                                 group by gi.codigo_trafo_mb, est_sum
                               ) b
                         where a.codigo_trafo_mb = b.codigo_trafo_mb
                           and a.conteo = b.conteo
                      ) X2,
                      (select codigo, sspdid from bdiv10_sgd_trafo_mb where onis_stat = '0' and onis_ver not like '%.%') Y2
                    where x2.codigo_trafo_mb = y2.codigo
                    minus
                    select y2.codigo, y2.sspdid,TRUNC(sysdate)
                    from 
                      (
                        select B.NIS_RAD,gi.codigo_trafo_mb
                           from
                           (
                            SELECT Y.NIS_RAD, Y.EST_SUM
                                FROM AUD_RECIBOS R, AUD_SUMCON Y
                              WHERE (Y.TIP_SERV = 'SV100' or Y.TIP_SERV ='SV120')
                                AND Y.EST_SUM = 'EC000' 
                                AND R.NIS_RAD   = Y.NIS_RAD
                                and r.F_PUESTA_COBRO>=to_date(to_char(add_months(sysdate,-2),'yyyymm')||'01 00:00:00','yyyymmdd hh24:mi:ss')--No se excluyen los que tiene facturas dos meses atras.
                                    and F_PUESTA_COBRO<=sysdate
                              GROUP BY Y.NIS_RAD, Y.EST_SUM
                           )b,
                           gi_cliente_instalacion gi
                           where b.nis_Rad=gi.nis_rad
                      ) X2,
                      (select codigo, sspdid from bdiv10_sgd_trafo_mb where onis_stat = '0' and onis_ver not like '%.%') Y2
                    where x2.codigo_trafo_mb = y2.codigo";
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TRAFOS_BAJA");
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 21,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "TRAFOS_BAJA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = x,
                },
                Agrupador = "Extraccion",
                IdGrupoEjecucion = 5,
                IdCategoria = 1
            });

            reporte = estructuras.FirstOrDefault(x => x.Nombre == "EXTR_XM_DIARIO_NF");
            archivo = archivos.FirstOrDefault(x => x.Nombre == "XM_DIARIO_CETSA_VERIFICACION_");
            consulta4 = @"SELECT  NULL CODIGO_EVENTO,NULL FECHA_INICIAL,NULL FECHA_FINAL,a.sspdid CODIGO_ELEMENTO,1 TIPO_ELEMENTO,NULL COD_CAUSA, NULL EVENTO_CONTINUA,NULL EVENTO_EXCL_ZNI,NULL AFECTA_CONEXION_GEN,NULL USERPUB                                  
                          FROM   bdiv10_sgd_trafo_mb a 
                                      WHERE a.onis_stat = '0'
                                         AND a.onis_ver NOT LIKE '%.%'
                                         AND a.nro_centro = 5000
                                         and NVL(SSPDID,'0') <> '0' --- Add(2018/02/02)
                                         and  a.CANT_CLIENTES > 0
                          			   AND EXISTS (select distinct ci.CODIGO_TRAFO_MB from aud_sumcon s , (select CODIGO_TRAFO_MB, nis_rad, max(f_actual) from gi_cliente_instalacion  group by CODIGO_TRAFO_MB, nis_rad) ci
                          				where s.nis_rad= ci.nis_rad
                          				  AND s.tip_suministro != 'SU035'
                          				  AND (s.tip_serv='SV100' OR s.tip_serv='SV120')
                          				  AND s.nic>0
                          				  AND a.codigo = ci.CODIGO_TRAFO_MB)";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 22,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "FXM_DIARIO_CETSA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                IdGrupoEjecucion = 5,
                IdCategoria = 1,
                ConsultaFinal = @"SELECT CODIGO_EVENTO,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,COD_CAUSA,EVENTO_CONTINUA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB 
                                  FROM TB_DIARIO_CETSA_EXTR_XM_DIARIO_NF C
                                  WHERE
                                    C.PERIODO_SIR = @periodo
                                  AND    C.EMPRESA = 'CETSA'
                                  AND    C.TIPO_ELEMENTO IN (1)
                                  UNION
                                  SELECT  'NA',NULL,NULL,a.CODIGO_ELEMENTO,a.TIPO_ELEMENTO,NULL,NULL,NULL,NULL,NULL
                                  FROM Temp_Combinacion_EXTR_XM_DIARIO_NF a
                                  left JOIN (SELECT c.CODIGO_ELEMENTO, c.PERIODO_SIR FROM TB_DIARIO_CETSA_EXTR_XM_DIARIO_NF C WHERE  C.PERIODO_SIR = @periodo AND C.EMPRESA = 'CETSA') b ON (a.codigo_elemento = b.codigo_elemento)
                                  WHERE a.CODIGO_ELEMENTO not IN (select a.SSPDID from [dbo].[TB_DIARIO_CETSA_TRAFOS_BAJA] a)
                                  AND b.codigo_elemento is null
                                  AND NOT EXISTS (SELECT SPT.CODIGO_CT FROM ParidadTrafo SPT WHERE CONVERT(VARCHAR,SPT.CODIGO_CT) = a.CODIGO_ELEMENTO AND SPT.ACCION ='Excluir')"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 23,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 5,
                IdArchivo = archivo.IdArchivo,
                Agrupador = "Extraccion",
                IdCategoria = 1
            });

            archivo = archivos.FirstOrDefault(x => x.Nombre == "XM_DIARIO_EPSA_VERIFICACION_");

            consulta4 = @"SELECT  NULL CODIGO_EVENTO,NULL FECHA_INICIAL,NULL FECHA_FINAL,a.sspdid CODIGO_ELEMENTO,1 TIPO_ELEMENTO,NULL COD_CAUSA, NULL EVENTO_CONTINUA,NULL EVENTO_EXCL_ZNI,NULL AFECTA_CONEXION_GEN,NULL USERPUB                                                                                                       
                          FROM   bdiv10_sgd_trafo_mb a 
                                      WHERE a.onis_stat = '0'
                                         AND a.onis_ver NOT LIKE '%.%'
                                         AND a.nro_centro = 4000
                                         and NVL(SSPDID,'0') <> '0'
                                         and  a.CANT_CLIENTES > 0
                          			   AND EXISTS (select distinct ci.CODIGO_TRAFO_MB from aud_sumcon s , (select CODIGO_TRAFO_MB, nis_rad, max(f_actual) from gi_cliente_instalacion  group by CODIGO_TRAFO_MB, nis_rad) ci
                          				where s.nis_rad= ci.nis_rad
                          				  AND s.tip_suministro != 'SU035'
                          				  AND (s.tip_serv='SV100' OR s.tip_serv='SV120')
                          				  AND s.nic>0
                          				  AND a.codigo = ci.CODIGO_TRAFO_MB)";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 24,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "FXM_DIARIO_EPSA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                IdGrupoEjecucion = 5,
                IdCategoria = 1,
                ConsultaFinal = @"SELECT CODIGO_EVENTO,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,COD_CAUSA,EVENTO_CONTINUA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB 
                                  FROM TB_DIARIO_CETSA_EXTR_XM_DIARIO_NF C
                                  WHERE
                                    C.PERIODO_SIR = @periodo
                                  AND    C.EMPRESA = 'EPSA'
                                  AND    C.TIPO_ELEMENTO IN (1)
                                  UNION
                                  SELECT  'NA',NULL,NULL,a.CODIGO_ELEMENTO,a.TIPO_ELEMENTO,NULL,NULL,NULL,NULL,NULL
                                  FROM Temp_Combinacion_EXTR_XM_DIARIO_NF a
                                  left JOIN (SELECT c.CODIGO_ELEMENTO, c.PERIODO_SIR FROM TB_DIARIO_CETSA_EXTR_XM_DIARIO_NF C WHERE  C.PERIODO_SIR = @periodo AND C.EMPRESA = 'EPSA') b ON (a.codigo_elemento = b.codigo_elemento)
                                  WHERE a.CODIGO_ELEMENTO not IN (select a.SSPDID from [dbo].[TB_DIARIO_CETSA_TRAFOS_BAJA] a)
                                  AND b.codigo_elemento is null
                                  AND NOT EXISTS (SELECT SPT.CODIGO_CT FROM ParidadTrafo SPT WHERE CONVERT(VARCHAR,SPT.CODIGO_CT) = a.CODIGO_ELEMENTO AND SPT.ACCION ='Excluir')"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 25,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 5,
                IdArchivo = archivo.IdArchivo,
                Agrupador = "Extraccion",
                IdCategoria = 1
            });


            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TRSF_XM_DIARIO_NF_HIST");

            consulta4 = @"SELECT I.CODIGO_EVENTO,I.FECHA_INICIAL,I.FECHA_FINAL,I.CODIGO_ELEMENTO,I.TIPO_ELEMENTO,I.EVENTO_CONTINUA,I.EMPRESA,I.PERIODO_SIR,I.[STR],I.CABECERA_MT,I.COD_CAUSA,I.EVENTO_EXCL_ZNI,I.AFECTA_CONEXION_GEN,I.USERPUB
                          FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF I
                          --WHERE I.PERIODO_SIR = @periodo AND I.IND_AGRUPADO = 1
                          ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 26,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 27,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });

            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TRSF_XM_DIARIO_NF");
            consulta4 = @"SELECT I.CODIGO_EVENTO,I.FECHA_INICIAL,I.FECHA_FINAL,I.CODIGO_ELEMENTO,I.TIPO_ELEMENTO,I.EVENTO_CONTINUA,I.EMPRESA,I.PERIODO_SIR,I.[STR],I.CABECERA_MT,I.COD_CAUSA,I.EVENTO_EXCL_ZNI,I.AFECTA_CONEXION_GEN,I.USERPUB INTO #tmpl
                          FROM (
                                SELECT G.CODIGO_EVENTO,G.FECHA_INICIAL,G.FECHA_FINAL,G.CODIGO_ELEMENTO,G.TIPO_ELEMENTO,G.EVENTO_CONTINUA,G.EMPRESA,G.PERIODO_SIR,G.[STR],G.CABECERA_MT,G.COD_CAUSA,G.EVENTO_EXCL_ZNI,G.AFECTA_CONEXION_GEN,G.USERPUB
                                  FROM (
                                        SELECT E.CODIGO_EVENTO,E.FECHA_INICIAL,E.FECHA_FINAL,E.CODIGO_ELEMENTO,E.TIPO_ELEMENTO,E.EVENTO_CONTINUA,E.EMPRESA,E.PERIODO_SIR,E.[STR],E.CABECERA_MT,E.COD_CAUSA,E.EVENTO_EXCL_ZNI,E.AFECTA_CONEXION_GEN,E.USERPUB
                                          FROM (
                                                SELECT C.CODIGO_EVENTO,C.FECHA_INICIAL,C.FECHA_FINAL,C.CODIGO_ELEMENTO,C.TIPO_ELEMENTO,C.EVENTO_CONTINUA,C.EMPRESA,C.PERIODO_SIR,C.[STR],C.CABECERA_MT,C.COD_CAUSA,C.EVENTO_EXCL_ZNI,C.AFECTA_CONEXION_GEN,C.USERPUB
                                                  FROM (
                                                        -- IDENTIFICA LAS INCIDENCIAS CON CAUSA EGENRICA Y NO LAS PASA PARA TRANSFORMACION.
                                                         SELECT A.CODIGO_EVENTO,A.FECHA_INICIAL,A.FECHA_FINAL,A.CODIGO_ELEMENTO,A.TIPO_ELEMENTO,A.EVENTO_CONTINUA,A.EMPRESA,A.PERIODO_SIR,A.[STR],A.CABECERA_MT,A.COD_CAUSA,A.EVENTO_EXCL_ZNI,A.AFECTA_CONEXION_GEN,A.USERPUB
                                                          FROM   (select distinct CODIGO_EVENTO,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,EVENTO_CONTINUA,EMPRESA,PERIODO_SIR,[STR],CABECERA_MT,COD_CAUSA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB
                                                                   from TB_DIARIO_CETSA_EXTR_XM_DIARIO_NF
                                                                 ) A
                                                          WHERE  A.PERIODO_SIR = @periodo
                                                          /*AND    A.FECHA_INICIAL IS NOT NULL -- NO REPORTAR INCIDENCIA SI TIENE FECHA FINAL VACIA
                                                          AND    A.FECHA_FINAL IS NOT NULL*/ -- NO REPORTAR INCIDENCIA SI TIENE FECHA FINAL VACIA
                                                          AND    NOT EXISTS (SELECT B.NRO_INCIDENCIA FROM [dbo].[TB_DIARIO_CETSA_XMD_INCIDENCIAS_GENERICAS] B
                                                                             WHERE  B.NRO_INCIDENCIA = A.CODIGO_EVENTO AND B.PERIODO_SIR = A.PERIODO_SIR
                                                                                    and b.IDCAUSA_SIR='4'  --4=No Reportar
                                                                            )
                                                      ) C
                                                WHERE NOT EXISTS (SELECT  D.NRO_INCIDENCIA
                                                                    FROM    TB_DIARIO_CETSA_XMD_INTERRUP_DUPLICADAS D
                                                                    WHERE   D.NRO_INCIDENCIA = C.CODIGO_EVENTO
                                                                      --AND   D.CODIGO_CT = C.CODIGO_ELEMENTO
                                                                      AND   D.F_ALTA = C.FECHA_INICIAL
                                                                      AND   D.F_REPOSICION = C.FECHA_FINAL
                                                                      AND   D.IDCAUSA_SIR ='4'
                                                                  )
                                               ) E
                                        WHERE NOT EXISTS (SELECT  F.NRO_INCIDENCIA
                                                            FROM    TB_DIARIO_CETSA_XMD_INTERRUP_DURACION_NEGATIVA F
                                                            WHERE   F.NRO_INCIDENCIA = E.CODIGO_EVENTO
                                                              --AND   F.CODIGO_CT = E.CODIGO_ELEMENTO
                                                              AND   F.F_ALTA = E.FECHA_INICIAL
                                                              AND   F.F_REPOSICION = E.FECHA_INICIAL
                                                              AND   F.IDCAUSA_SIR='4'
                                                          )
                                       ) G
                                WHERE  NOT EXISTS (SELECT  H.NRO_INCIDENCIA
                                                    FROM    TB_DIARIO_CETSA_XMD_INTERRUP_MAYOR_OCHO_HORAS H
                                                    WHERE   H.NRO_INCIDENCIA = G.CODIGO_EVENTO
                                                      --AND   H.CODIGO_CT = G.CODIGO_ELEMENTO
                                                      AND   H.F_ALTA = G.FECHA_INICIAL
                                                      AND   H.F_REPOSICION = G.FECHA_INICIAL
                                                      AND   H.IDCAUSA_SIR='4'
                                                  )
                               ) I;
                    
                    
                    	UPDATE t0 SET FECHA_FINAL = REG.FECHA_FINAL,ind_agrupado = 1,EVENTO_CONTINUA = REG.EVENTO_CONTINUA
                    	FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF t0
                    	INNER JOIN (SELECT * 
                    				FROM (SELECT *,CONVERT(DATETIME,LEFT(FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(FECHA_FINAL,8),8) D_FECHA_FINAL,
                    				   MAX(CONVERT(DATETIME,LEFT(FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(FECHA_FINAL,8),8)) OVER(PARTITION BY CODIGO_EVENTO,codigo_elemento,PERIODO_SIR)  v_fechas_comp 
                    		  FROM #tmpl)a) REG ON CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8) = v_fechas_comp
                              AND  REG.CODIGO_EVENTO = t0.CODIGO_EVENTO
                              AND REG.codigo_elemento = t0.codigo_elemento
                              AND REG.PERIODO_SIR = T0.PERIODO_SIR							  
                    	AND NOT (v_fechas_comp = CONVERT(DATETIME,'29991231')
                    	OR (DATEDIFF(DAY,D_FECHA_FINAL, v_fechas_comp))> 120 
                    	OR (DATEDIFF(DAY,D_FECHA_FINAL, v_fechas_comp)) < 0)
                    
                    
                    	SELECT I.CODIGO_EVENTO,I.FECHA_INICIAL,I.FECHA_FINAL,I.CODIGO_ELEMENTO,I.TIPO_ELEMENTO,I.EVENTO_CONTINUA,I.EMPRESA,I.PERIODO_SIR,I.[STR],I.CABECERA_MT,I.COD_CAUSA,I.EVENTO_EXCL_ZNI,I.AFECTA_CONEXION_GEN,I.USERPUB,0 IND_AGRUPADO
                        FROM(SELECT * 
                        			FROM (SELECT *,CONVERT(DATETIME,LEFT(FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(FECHA_FINAL,8),8) D_FECHA_FINAL,
                        			   MAX(CONVERT(DATETIME,LEFT(FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(FECHA_FINAL,8),8)) OVER(PARTITION BY CODIGO_EVENTO,codigo_elemento,PERIODO_SIR)  v_fechas_comp 
                        	  FROM #tmpl) a ) I
                        WHERE (v_fechas_comp = CONVERT(DATETIME,'29991231')
                        OR (DATEDIFF(DAY,D_FECHA_FINAL, v_fechas_comp))> 120 
                        OR (DATEDIFF(DAY,D_FECHA_FINAL, v_fechas_comp)) < 0)";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 28,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 28,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id,
                IdGrupoEjecucion = 6,
                //IdArchivo ,
                Agrupador = "Transformacio",
                IdCategoria = 1
            });

            

            consulta4 = @"SELECT  NULL CODIGO_EVENTO,NULL FECHA_INICIAL,NULL FECHA_FINAL,a.sspdid CODIGO_ELEMENTO,1 TIPO_ELEMENTO,NULL COD_CAUSA, NULL EVENTO_CONTINUA,NULL EVENTO_EXCL_ZNI,NULL AFECTA_CONEXION_GEN,NULL USERPUB
                                  ,null IND_AGRUPADO
                          FROM   bdiv10_sgd_trafo_mb a 
                                      WHERE a.onis_stat = '0'
                                         AND a.onis_ver NOT LIKE '%.%'
                                         AND a.nro_centro IN (4000,5000)
                                         and NVL(SSPDID,'0') <> '0'
                                         and  a.CANT_CLIENTES > 0
                          			   AND EXISTS (select distinct ci.CODIGO_TRAFO_MB from aud_sumcon s , (select CODIGO_TRAFO_MB, nis_rad, max(f_actual) from gi_cliente_instalacion  group by CODIGO_TRAFO_MB, nis_rad) ci
                          				where s.nis_rad= ci.nis_rad
                          				  AND s.tip_suministro != 'SU035'
                          				  AND (s.tip_serv='SV100' OR s.tip_serv='SV120')
                          				  AND s.nic>0
                          				  AND a.codigo = ci.CODIGO_TRAFO_MB)";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 29,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_XM_VACIOS_NF",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                IdGrupoEjecucion = 5,
                IdCategoria = 1,
                ConsultaFinal = @"INSERT INTO [dbo].[TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF] 
								  (CODIGO_EVENTO, FECHA_INICIAL, FECHA_FINAL, CODIGO_ELEMENTO, TIPO_ELEMENTO, EVENTO_CONTINUA, EMPRESA,PERIODO_SIR, [STR], CABECERA_MT, COD_CAUSA, EVENTO_EXCL_ZNI, AFECTA_CONEXION_GEN, USERPUB,IND_AGRUPADO)
                                  SELECT  'NA',NULL,NULL,a.CODIGO_ELEMENTO,a.TIPO_ELEMENTO,NULL,NULL,@periodo,NULL,NULL,NULL,NULL,NULL,NULL,NULL
                                  FROM Temp_Combinacion_TRSF_XM_DIARIO_NF a
                                  left JOIN [dbo].[TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF] b ON (a.codigo_elemento = b.codigo_elemento AND b.PERIODO_SIR = @periodo)
                                  WHERE a.CODIGO_ELEMENTO not IN (select a.SSPDID from [dbo].[TB_DIARIO_CETSA_TRAFOS_BAJA] a)
                                  AND b.codigo_elemento is null
                                  AND NOT EXISTS (SELECT SPT.CODIGO_CT FROM ParidadTrafo SPT WHERE CONVERT(VARCHAR,SPT.CODIGO_CT) = a.CODIGO_ELEMENTO AND SPT.ACCION ='Excluir')"
            });

            consulta4 = @"SELECT t0.CODIGO_EVENTO,t0.CODIGO_ELEMENTO,t0.PERIODO_SIR,CONVERT(DATETIME,LEFT(REG.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(REG.FECHA_INICIAL,8),8) FECHA_INICIAL,CONVERT(DATETIME,LEFT(REG.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(REG.FECHA_FINAL,8),8) FECHA_FINAL,COUNT(*) V_REG_AGRUP  INTO #C_TRSF_XM_DIARIO_NF
                            FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF_HIST t0
	                        INNER JOIN TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF REG
                            ON  t0.CODIGO_EVENTO = REG.CODIGO_EVENTO
                            AND t0.CODIGO_ELEMENTO = REG.CODIGO_ELEMENTO
                            AND t0.PERIODO_SIR = REG.PERIODO_SIR
                            AND CONVERT(DATETIME,LEFT(t0.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_INICIAL,8),8) >= CONVERT(DATETIME,LEFT(REG.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(REG.FECHA_INICIAL,8),8)
                            AND CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8) <= CONVERT(DATETIME,LEFT(REG.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(REG.FECHA_FINAL,8),8)
                            AND DATEDIFF(HOUR,CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8),CONVERT(DATETIME,LEFT(REG.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(REG.FECHA_INICIAL,8),8)) > 180
	                        GROUP BY t0.CODIGO_EVENTO,t0.CODIGO_ELEMENTO,t0.PERIODO_SIR,REG.FECHA_INICIAL,REG.FECHA_FINAL;

	                        INSERT INTO TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF (CODIGO_EVENTO,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,EVENTO_CONTINUA,EMPRESA,PERIODO_SIR,[STR],CABECERA_MT,COD_CAUSA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB)
                               /*SELECT REG.CODIGO_EVENTO,REG.FECHA_INICIAL,REG.FECHA_FINAL,REG.CODIGO_ELEMENTO,REG.TIPO_ELEMENTO,REG.EVENTO_CONTINUA,REG.EMPRESA,REG.PERIODO,REG.STR,REG.CABECERA_MT,REG.COD_CAUSA,REG.EVENTO_EXCL_ZNI,REG.AFECTA_CONEXION_GEN,REG.USERPUB*/
                               SELECT t0.CODIGO_EVENTO, t0.FECHA_INICIAL, t0.FECHA_FINAL, t0.CODIGO_ELEMENTO, TIPO_ELEMENTO, EVENTO_CONTINUA, EMPRESA, t0.PERIODO_SIR, STR, CABECERA_MT, COD_CAUSA, EVENTO_EXCL_ZNI, AFECTA_CONEXION_GEN, USERPUB
                               FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF_HIST t0
	                        INNER JOIN #C_TRSF_XM_DIARIO_NF REG ON t0.CODIGO_EVENTO = REG.CODIGO_EVENTO
                               AND t0.CODIGO_ELEMENTO = REG.CODIGO_ELEMENTO
                               AND t0.PERIODO_SIR = REG.PERIODO_SIR
                               AND CONVERT(DATETIME,LEFT(t0.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_INICIAL,8),8) >= REG.FECHA_INICIAL
                               AND CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8) <= REG.FECHA_FINAL
                               AND DATEDIFF(HOUR,CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8),REG.FECHA_INICIAL) > 180
	                        AND REG.V_REG_AGRUP <= 0
	                        
	                        DELETE t0 FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF t0
                               INNER JOIN #C_TRSF_XM_DIARIO_NF REG ON t0.CODIGO_EVENTO = REG.CODIGO_EVENTO
                               AND t0.CODIGO_ELEMENTO = REG.CODIGO_ELEMENTO
                               AND t0.PERIODO_SIR = REG.PERIODO_SIR
                               AND CONVERT(DATETIME,LEFT(t0.FECHA_INICIAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_INICIAL,8),8) = REG.FECHA_INICIAL
                               AND CONVERT(DATETIME,LEFT(t0.FECHA_FINAL,10),103)+CONVERT(DATETIME,RIGHT(t0.FECHA_FINAL,8),8) = REG.FECHA_FINAL        
	                        AND REG.V_REG_AGRUP <= 0                            
                            
                            SELECT * FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF_HIST t0 WHERE 1 = 0
                          ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 30,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });
            
            archivo = archivos.FirstOrDefault(x => x.Nombre == "XM_DIARIO_EPSA_");
            consulta4 = @"SELECT CASE C.CODIGO_EVENTO WHEN 'NA' THEN rtrim(C.CODIGO_EVENTO) ELSE rtrim(CODIGO_EVENTO) END,'' codigo_bloque,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,COD_CAUSA,EVENTO_CONTINUA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB
	                      FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF C
	                      WHERE C.PERIODO_SIR = @periodo
	                      AND    C.EMPRESA = 'EPSA' 
	                      AND    C.TIPO_ELEMENTO IN (1)
	                      ORDER BY CODIGO_EVENTO DESC, CODIGO_ELEMENTO, FECHA_INICIAL ASC, FECHA_FINAL
                         ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 31,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 32,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4,
                IdArchivo = archivo.IdArchivo
            });

            archivo = archivos.FirstOrDefault(x => x.Nombre == "XM_DIARIO_CETSA_");
            consulta4 = @"SELECT CASE C.CODIGO_EVENTO WHEN 'NA' THEN rtrim(C.CODIGO_EVENTO) ELSE rtrim(CODIGO_EVENTO) END,'' codigo_bloque,FECHA_INICIAL,FECHA_FINAL,CODIGO_ELEMENTO,TIPO_ELEMENTO,COD_CAUSA,EVENTO_CONTINUA,EVENTO_EXCL_ZNI,AFECTA_CONEXION_GEN,USERPUB
	                      FROM TB_DIARIO_CETSA_TRSF_XM_DIARIO_NF C
	                      WHERE C.PERIODO_SIR = @periodo
	                      AND    C.EMPRESA = 'CETSA' 
	                      AND    C.TIPO_ELEMENTO IN (1)
	                      ORDER BY CODIGO_EVENTO DESC, CODIGO_ELEMENTO, FECHA_INICIAL ASC, FECHA_FINAL
                         ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 33,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                ConsultaFinal = consulta4
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 34,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = reporte.Id, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura                
                IdGrupoEjecucion = 6,
                IdCategoria = 1,
                IdArchivo = archivo.IdArchivo                
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            //registrar un flujo de trabajo
            registro.nuevo();
            context.Registrar(registro);

            //Ejecutar el flujo creado, compruebe el identificador
            //context.Ejecutar(new MODFlujoFiltro
            //{
            //    IdEmpresa = 1035,
            //    IdServicio = 2,
            //    IdElemento = 2,
            //    TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
            //    Periodo = new DateTime(2021, 1, 1),
            //    Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.diario,
            //    IdGrupoEjecucion = 1
            //});
        }
        static void CrearEstructurasDiario()
        {
            MODReporte estructura = new MODReporte()
            {
                Descripcion = "Extraer interrupciones",
                Nombre = "XMD_INTERRUPCIONES",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "NRO_INCIDENCIA",
                        nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
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
                        Nombre = "COD_CIRCUITO",
                        nombreVisible = "COD_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "DESCRIPCION_CIRCUITO",
                        nombreVisible = "DESCRIPCION_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
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
                        Nombre = "SECTOR",
                        nombreVisible = "SECTOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
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
                        Nombre = "CODIGO",
                        nombreVisible = "CODIGO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "GRUPO",
                        nombreVisible = "GRUPO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                    ,
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "MATRICULA_TRAFO",
                        nombreVisible = "MATRICULA_TRAFO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "11",
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
                        Nombre = "MATRICULA_CT_APOYO",
                        nombreVisible = "MATRICULA_CT_APOYO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
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
                        Nombre = "CAPACIDAD",
                        nombreVisible = "CAPACIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "8,1",
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
                        Nombre = "DEM_ENERGIA",
                        nombreVisible = "DEM_ENERGIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "CANT_CLIENTES",
                        nombreVisible = "CANT_CLIENTES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "LONGITUD",
                        nombreVisible = "LONGITUDO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "14,7",
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
                        Nombre = "LATITUD",
                        nombreVisible = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "14,7",
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
                        Nombre = "ALTITUD",
                        nombreVisible = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10,3",
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
                        Nombre = "PROPIEDAD",
                        nombreVisible = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
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
                        Largo = "25",
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
                        Nombre = "TIPO_CT",
                        nombreVisible = "TIPO_CT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "DESCRIPCION",
                        nombreVisible = "DESCRIPCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 17,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "DURACION",
                        nombreVisible = "DURACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
                        Ordinal = 18,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_REPOSICION",
                        nombreVisible = "F_REPOSICION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "7",
                        Ordinal = 19,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_ALTA",
                        nombreVisible = "F_ALTA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "7",
                        Ordinal = 21,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "CAUSA_DESC",
                        nombreVisible = "CAUSA_DESC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 21,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "SUBFAMILIA_CAUSA",
                        nombreVisible = "SUBFAMILIA_CAUSA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 22,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "INC_AGRUPADA",
                        nombreVisible = "INC_AGRUPADA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 23,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "USUARIO",
                        nombreVisible = "USUARIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 24,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "OBSERVACION",
                        nombreVisible = "OBSERVACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2000",
                        Ordinal = 25,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "DESC_INCIDENCIA",
                        nombreVisible = "DESC_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
                        Ordinal = 26,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "INCIDENCIA_MANIO_APERT",
                        nombreVisible = "INCIDENCIA_MANIO_APERT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
                        Ordinal = 28,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "INCIDENCIA_MANIO_CIERRE",
                        nombreVisible = "INCIDENCIA_MANIO_CIERRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
                        Ordinal = 30,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "MATRICULA_CT_APOYO",
                        nombreVisible = "MATRICULA_CT_APOYO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
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
                        Nombre = "CODIGO_CT",
                        nombreVisible = "CODIGO_CT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
                        Ordinal = 30,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_ACTUAL",
                        nombreVisible = "F_ACTUAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "15",
                        Ordinal = 31,
                        esVersionable = false,
                        esEditable = false,
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //
            //estructura = new MODReporte()
            //{
            //    Descripcion = "Extraer incidencias",
            //    Nombre = "XMD_INCIDENCIAS",
            //    IdCategoria = idCategoriaSUI,
            //    EsReporte = false,
            //    Activo = true,
            //    ActivoEmpresa = true,
            //    IdEmpresa = idEmpresa,
            //    IdServicio = idServicio,
            //
            //    campos = new List<MODCampos>()
            //    {
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "CODIGO",
            //            nombreVisible = "CODIGO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "8",
            //            Ordinal = 0,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NOMBRE_CIRCUITO",
            //            nombreVisible = "NOMBRE_CIRCUITO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "30",
            //            Ordinal = 1,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NRO_INCIDENCIA",
            //            nombreVisible = "NRO_INCIDENCIA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "11",
            //            Ordinal = 2,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "SUBESTACION",
            //            nombreVisible = "SUBESTACION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "30",
            //            Ordinal = 3,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "SECTOR",
            //            nombreVisible = "SECTOR",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 4,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "USUARIOS",
            //            nombreVisible = "USUARIOS",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 5,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //        ,
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DEMANADA",
            //            nombreVisible = "DEMANADA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 6,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "VOLTAJE",
            //            nombreVisible = "VOLTAJE",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 7,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "GRUPO",
            //            nombreVisible = "GRUPO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 8,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "LONGITUD_RED",
            //            nombreVisible = "LONGITUD_RED",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 9,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "RELE_RECIERRE",
            //            nombreVisible = "RELE_RECIERRE",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 10,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "ALIMENTADOR_RADIAL",
            //            nombreVisible = "ALIMENTADOR_RADIAL",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 11,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NORMAL_ABIERTO",
            //            nombreVisible = "NORMAL_ABIERTO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 12,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "LONGITUD",
            //            nombreVisible = "LONGITUD",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "18,8",
            //            Ordinal = 13,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "LATITUD",
            //            nombreVisible = "LATITUD",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "7,3",
            //            Ordinal = 14,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "ALTITUD",
            //            nombreVisible = "ALTITUD",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "8,2",
            //            Ordinal = 15,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "PROPIEDAD",
            //            nombreVisible = "PROPIEDAD",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "1",
            //            Ordinal = 16,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "KM_SUBT",
            //            nombreVisible = "KM_SUBT",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 17,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "KM_AEREA",
            //            nombreVisible = "KM_AEREA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 18,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "EMPRESA",
            //            nombreVisible = "EMPRESA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "20",
            //            Ordinal = 19,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DESCRIPCION",
            //            nombreVisible = "DESCRIPCION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 21,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "SUBFAMILIA_CAUSA",
            //            nombreVisible = "SUBFAMILIA_CAUSA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 22,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DURACION",
            //            nombreVisible = "DURACION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 23,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_REPOSICION",
            //            nombreVisible = "F_REPOSICION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "7",
            //            Ordinal = 24,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_ALTA",
            //            nombreVisible = "F_ALTA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "15",
            //            Ordinal = 25,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DESC_INCIDENCIA",
            //            nombreVisible = "DESC_INCIDENCIA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "80",
            //            Ordinal = 26,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "CAUSA_DESC",
            //            nombreVisible = "CAUSA_DESC",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "60",
            //            Ordinal = 27,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "OBSERVACION",
            //            nombreVisible = "OBSERVACION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "2000",
            //            Ordinal = 28,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "POT_AFECTADA",
            //            nombreVisible = "POT_AFECTADA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "12,1",
            //            Ordinal = 30,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //    }
            //};
            //contextReporte.Registrar(estructura);
            //
            //estructura = new MODReporte()
            //{
            //    Descripcion = "Negativas",
            //    Nombre = "XMD_INTERRUP_DURACION_NEGATIVA",
            //    IdCategoria = idCategoriaSUI,
            //    EsReporte = false,
            //    Activo = true,
            //    ActivoEmpresa = true,
            //    IdEmpresa = idEmpresa,
            //    IdServicio = idServicio,
            //
            //    campos = new List<MODCampos>()
            //    {
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NRO_INCIDENCIA",
            //            nombreVisible = "NRO_INCIDENCIA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "7",
            //            Ordinal = 0,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_REPOSICION",
            //            nombreVisible = "F_REPOSICION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "30",
            //            Ordinal = 1,
            //            esVersionable = false,
            //            esEditable = true,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_ALTA",
            //            nombreVisible = "F_ALTA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "11",
            //            Ordinal = 2,
            //            esVersionable = false,
            //            esEditable = true,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "INCIDENCIA_MANIO_APERT",
            //            nombreVisible = "INCIDENCIA_MANIO_APERT",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 3,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "INCIDENCIA_MANIO_CIERRE",
            //            nombreVisible = "INCIDENCIA_MANIO_CIERRE",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 4,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "MATRICULA_CT_APOYO",
            //            nombreVisible = "MATRICULA_CT_APOYO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "15",
            //            Ordinal = 5,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //        ,
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "CODIGO_CT",
            //            nombreVisible = "CODIGO_CT",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "8",
            //            Ordinal = 6,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_ACTUAL",
            //            nombreVisible = "F_ACTUAL",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "50",
            //            Ordinal = 7,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //    }
            //};
            //contextReporte.Registrar(estructura);
            //
            //estructura = new MODReporte()
            //{
            //    Descripcion = "Interrupción generica",
            //    Nombre = "XMD_INCIDENCIAS_GENERICAS",
            //    IdCategoria = idCategoriaSUI,
            //    EsReporte = false,
            //    Activo = true,
            //    ActivoEmpresa = true,
            //    IdEmpresa = idEmpresa,
            //    IdServicio = idServicio,
            //
            //    campos = new List<MODCampos>()
            //    {
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NRO_INCIDENCIA",
            //            nombreVisible = "NRO_INCIDENCIA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "8",
            //            Ordinal = 0,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //    }
            //};
            //contextReporte.Registrar(estructura);
            //
            //estructura = new MODReporte()
            //{
            //    Descripcion = "Ramales y Trafos",
            //    Nombre = "XMD_INTERRUP_MAYOR_OCHO_HORAS",
            //    IdCategoria = idCategoriaSUI,
            //    EsReporte = false,
            //    Activo = true,
            //    ActivoEmpresa = true,
            //    IdEmpresa = idEmpresa,
            //    IdServicio = idServicio,
            //
            //    campos = new List<MODCampos>()
            //    {
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "NRO_INCIDENCIA",
            //            nombreVisible = "NRO_INCIDENCIA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "8",
            //            Ordinal = 0,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DESCRIPCION_CIRCUITO",
            //            nombreVisible = "DESCRIPCION_CIRCUITO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "30",
            //            Ordinal = 1,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "SECTOR",
            //            nombreVisible = "SECTOR",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "20",
            //            Ordinal = 2,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DESCRIPCION",
            //            nombreVisible = "DESCRIPCION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 3,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "DURACION",
            //            nombreVisible = "DURACION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 4,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_REPOSICION",
            //            nombreVisible = "F_REPOSICION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "18,8",
            //            Ordinal = 5,
            //            esVersionable = false,
            //            esEditable = true,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //        ,
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_ALTA",
            //            nombreVisible = "F_ALTA",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "18,8",
            //            Ordinal = 6,
            //            esVersionable = false,
            //            esEditable = true,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "USUARIO",
            //            nombreVisible = "USUARIO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "50",
            //            Ordinal = 7,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "OBSERVACION",
            //            nombreVisible = "OBSERVACION",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "2000",
            //            Ordinal = 8,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "INCIDENCIA_MANIO_APER",
            //            nombreVisible = "INCIDENCIA_MANIO_APER",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 9,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "INCIDENCIA_MANIO_CIERRE",
            //            nombreVisible = "RELE_RECIERRE",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
            //            Largo = "18,8",
            //            Ordinal = 10,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "MATRICULA_CT_APOYO",
            //            nombreVisible = "MATRICULA_CT_APOYO",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
            //            Largo = "15",
            //            Ordinal = 11,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "CODIGO_CT",
            //            nombreVisible = "CODIGO_CT",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
            //            Largo = "8",
            //            Ordinal = 12,
            //            esVersionable = false,
            //            esEditable = false,
            //
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        },
            //        new MODCampos()
            //        {
            //            Id = 0,
            //            IdReporte = 0,
            //            Nombre = "F_ACTUAL",
            //            nombreVisible = "F_ACTUAL",
            //            Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
            //            Largo = "18",
            //            Ordinal = 13,
            //            esVersionable = false,
            //            esEditable = false,
            //            IdEmpresa = idEmpresa,
            //            IdServicio = idServicio
            //        }
            //    }
            //};
            //contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Duplicadas",
                Nombre = "XMD_INTERRUP_DUPLICADAS",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "NRO_INCIDENCIA",
                        nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
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
                        Nombre = "CODIGO",
                        nombreVisible = "CODIGO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_ALTA",
                        nombreVisible = "F_ALTA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "11",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_REPOSICION",
                        nombreVisible = "F_REPOSICION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "8",
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
                        Nombre = "DURACION",
                        nombreVisible = "DURACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "IND_AGRUPADA",
                        nombreVisible = "IND_AGRUPADA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "15",
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
                        Nombre = "USUARIO",
                        nombreVisible = "USUARIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
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
                        Nombre = "MATRICULA_TRAFO",
                        nombreVisible = "MATRICULA_TRAFO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "11",
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
                        Nombre = "MATRICULA_CT_APOYO",
                        nombreVisible = "MATRICULA_CT_APOYO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
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
                        Nombre = "OBSERVACIONES",
                        nombreVisible = "OBSERVACIONES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "500",
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
                        Nombre = "INCIDENCIA_MANIO_APERT",
                        nombreVisible = "INCIDENCIA_MANIO_APERT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "INCIDENCIA_MANIO_CIERRE",
                        nombreVisible = "INCIDENCIA_MANIO_CIERRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "CODIGO_CT",
                        nombreVisible = "CODIGO_CT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "500",
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
                        Nombre = "F_ACTUAL",
                        nombreVisible = "F_ACTUAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "500",
                        Ordinal = 14,
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
                Descripcion = "Avisos Pendientes",
                Nombre = "XM_AVISOS_PENDIENTES",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                     new MODCampos(){Id = 0,IdReporte = 0,Nombre = "AVISO",nombreVisible = "AVISO",
                         Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "10",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "USUARIO",nombreVisible = "USUARIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "30",Ordinal = 1,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NIS_RAD",nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "7",Ordinal = 2,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NOMBRE",nombreVisible = "NOMBRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "30",Ordinal = 3,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "APELLIDOS",nombreVisible = "APELLIDOS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "30",Ordinal = 4,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "FECHA_INGRESO_AVISO",nombreVisible = "FECHA_INGRESO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,Largo = "7",Ordinal = 5,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NRO_OT",nombreVisible = "NRO_OT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "6",Ordinal = 6,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "ZONA",nombreVisible = "ZONA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "25",Ordinal = 7,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NRO_INCIDENCIA",nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "7",Ordinal = 8,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "DEPTO",nombreVisible = "DEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "30",Ordinal = 9,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "MUNICIPIO",nombreVisible = "MUNICIPIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "30",Ordinal = 10,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "CORREGIMIENTO",nombreVisible = "CORREGIMIENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string, Largo = "30",Ordinal = 11,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "LOCALIDAD",nombreVisible = "LOCALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "50",Ordinal = 12,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "FECHA_RESUELTO_AVISO",nombreVisible = "FECHA_RESUELTO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,Largo = "7",Ordinal = 13,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "OBSERVACION_TELEGESTIONES",nombreVisible = "OBSERVACION_TELEGESTIONES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "50",Ordinal = 14,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "DIRECCION",nombreVisible = "DIRECCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "60",Ordinal = 15,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NIC",nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "7",Ordinal = 16,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "PRIORIDAD",nombreVisible = "PRIORIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "2",Ordinal = 17,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "TIEMPO_TOTAL_HORAS",nombreVisible = "TIEMPO_TOTAL_HORAS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,Largo = "18,8",Ordinal = 18,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "FECHA_HHMMSS_INGRESO_AVISO",nombreVisible = "FECHA_HHMMSS_INGRESO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,Largo = "7",Ordinal = 19,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "FECHA_HHMMSS_RESUELTO_AVISO",nombreVisible = "FECHA_HHMMSS_RESUELTO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,Largo = "7",Ordinal = 20,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NOMBRE_USUARIO",nombreVisible = "NOMBRE_USUARIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "25",Ordinal = 21,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "TIPO_AVISO",nombreVisible = "TIPO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "80",Ordinal = 21,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "ESTADO_AVISO",nombreVisible = "ESTADO_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "50",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "CLASE_AVISO",nombreVisible = "CLASE_AVISO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "50",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "CLIENTE_VIP",nombreVisible = "CLIENTE_VIP",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "20",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "COD_CLIENTE_VIP",nombreVisible = "COD_CLIENTE_VIP",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,Largo = "20",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NRO_LLAMADAS",nombreVisible = "NRO_LLAMADAS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "7",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                    ,new MODCampos(){Id = 0,IdReporte = 0,Nombre = "NRO_LLAMADAS",nombreVisible = "NRO_LLAMADAS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,Largo = "7",Ordinal = 0,esVersionable = false,esEditable = false,IdEmpresa = idEmpresa,IdServicio = idServicio}
                }
            };
            contextReporte.Registrar(estructura);
            /*estructura = new MODReporte()
            {
                Descripcion = "Trafos Abiertos",
                Nombre = "XM_TRAFOS_ABIERTOS",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "NRO_INCIDENCIA",
                        nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
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
                        Nombre = "F_ALTA",
                        nombreVisible = "F_ALTA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "11",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_CIRCUITO",
                        nombreVisible = "COD_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "TRANSFORMADOR",
                        nombreVisible = "TRANSFORMADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //estructura = new MODReporte()
            {
                Descripcion = "Todas las interrucciones",
                Nombre = "TODAS_INTERRUCCIONES",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "ACCION",
                        nombreVisible = "ACCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
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
                        Nombre = "NRO_INCIDENCIA",
                        nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "11",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "F_ALTA",
                        nombreVisible = "F_ALTA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
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
                        Nombre = "F_REPOSICION",
                        nombreVisible = "F_REPOSICION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NOMBRE",
                        nombreVisible = "NOMBRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
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
                        Nombre = "CT_SALMT",
                        nombreVisible = "CT_SALMT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "25",
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
                        Nombre = "F_ALTA_NVA",
                        nombreVisible = "F_ALTA_NVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "F_ACTUAL",
                        nombreVisible = "F_ACTUAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "NRO_INCIDENCIA_NVA",
                        nombreVisible = "NRO_INCIDENCIA_NVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "MANIOBRA_APERT",
                        nombreVisible = "MANIOBRA_APERT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "MANIOBRA_CIERRE",
                        nombreVisible = "MANIOBRA_CIERRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "MANIOBRA_APERT_NVA",
                        nombreVisible = "MANIOBRA_APERT_NVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
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
                        Nombre = "MANIOBRA_CIERRE_NVA",
                        nombreVisible = "MANIOBRA_CIERRE_NVA ",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "25",
                        Ordinal = 13,
                        esVersionable = false,
                        esEditable = false,
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //estructura = new MODReporte()
            {
                Descripcion = "Todas las maniobras",
                Nombre = "TODAS_MANIOBRAS",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "ACCION",
                        nombreVisible = "ACCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
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
                        Nombre = "NRO_INCIDENCIA",
                        nombreVisible = "NRO_INCIDENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "11",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = true,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_MANIOBRA",
                        nombreVisible = "COD_MANIOBRA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
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
                        Nombre = "F_MANIOBRA",
                        nombreVisible = "F_MANIOBRA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NRO_INCIDENCIA_NVA",
                        nombreVisible = "NRO_INCIDENCIA_NVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
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
                        Nombre = "F_MANIOBRA_NVA",
                        nombreVisible = "F_MANIOBRA_NVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "25",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //
            //estructura = new MODReporte()
            {
                Descripcion = "Extraccion proceso diario",
                Nombre = "EXTR_XM_DIARIO_NF",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "CODIGO_EVENTO",
                        nombreVisible = "CODIGO_EVENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "11",
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
                        Nombre = "FECHA_INICIAL",
                        nombreVisible = "FECHA_INICIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
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
                        Nombre = "FECHA_FINAL",
                        nombreVisible = "FECHA_FINAL",
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
                        Nombre = "CODIGO_ELEMENTO",
                        nombreVisible = "CODIGO_ELEMENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "TIPO_ELEMENTO",
                        nombreVisible = "TIPO_ELEMENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "EVENTO_CONTINUA",
                        nombreVisible = "EVENTO_CONTINUA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                    ,
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
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
                        Nombre = "STR",
                        nombreVisible = "STR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "CABECERA_MT",
                        nombreVisible = "CABECERA_MT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "COD_CAUSA",
                        nombreVisible = "COD_CAUSA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
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
                        Nombre = "EVENTO_EXCL_ZNI",
                        nombreVisible = "EVENTO_EXCL_ZNI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "AFECTA_CONEXION_GEN",
                        nombreVisible = "AFECTA_CONEXION_GEN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "USERPUB",
                        nombreVisible = "USERPUB",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 12,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //estructura = new MODReporte()
            {
                Descripcion = "Trafos Baja",
                Nombre = "TRAFOS_BAJA",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "CODIGO_BDI",
                        nombreVisible = "CODIGO_BDI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "SSPDID",
                        nombreVisible = "SSPDID",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
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
                        Nombre = "FECHA",
                        nombreVisible = "FECHA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);
            //estructura = new MODReporte()
            {
                Descripcion = "Extraccion proceso diario",
                Nombre = "TRSF_XM_DIARIO_NF",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
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
                        Nombre = "CODIGO_EVENTO",
                        nombreVisible = "CODIGO_EVENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "11",
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
                        Nombre = "FECHA_INICIAL",
                        nombreVisible = "FECHA_INICIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
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
                        Nombre = "FECHA_FINAL",
                        nombreVisible = "FECHA_FINAL",
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
                        Nombre = "CODIGO_ELEMENTO",
                        nombreVisible = "CODIGO_ELEMENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "TIPO_ELEMENTO",
                        nombreVisible = "TIPO_ELEMENTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "EVENTO_CONTINUA",
                        nombreVisible = "EVENTO_CONTINUA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                    ,
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
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
                        Nombre = "STR",
                        nombreVisible = "STR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "CABECERA_MT",
                        nombreVisible = "CABECERA_MT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
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
                        Nombre = "COD_CAUSA",
                        nombreVisible = "COD_CAUSA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
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
                        Nombre = "EVENTO_EXCL_ZNI",
                        nombreVisible = "EVENTO_EXCL_ZNI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "AFECTA_CONEXION_GEN",
                        nombreVisible = "AFECTA_CONEXION_GEN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "USERPUB",
                        nombreVisible = "USERPUB",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 12,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                    ,
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "IND_AGRUPADO",
                        nombreVisible = "IND_AGRUPADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "50",
                        Ordinal = 13,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            //contextReporte.Registrar(estructura);*/
            ConfiguracionNegocio.RefrescarConfiguracion(SIR.Comun.Enumeradores.EnumTipoConfiguracion.reportes);

        }
        static void CrearArchivosLACDiario()
        {
            var estructuras = ObtenerEstructuras();
            int idReporteEPSA = 47;
            int idReporteCETSA = 48;
            MODReporte reporte = estructuras.FirstOrDefault(x => x.Nombre == "TODAS_INTERRUCCIONES");

            MODArchivo OpenAsociaInt = new MODArchivo()
            {
                IdReporte = reporte.Id,
                Nombre = "INTERRUPCIONES",
                Descripcion = "Exportar interrupciones a csv OPENASOCIA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
                Campos = new List<MODCamposArchivo>()               //
            };
            reporte.campos.ForEach(x =>
            {
                OpenAsociaInt.Campos.Add(new MODCamposArchivo { IdCampo = x.Id, Orden = x.Ordinal });
            });

            reporte = estructuras.FirstOrDefault(x => x.Nombre == "TODAS_MANIOBRAS");
            MODArchivo OpenAsociaMan = new MODArchivo()
            {
                IdReporte = reporte.Id,
                Nombre = "MANIOBRAS",
                Descripcion = "Exportar interrupciones a csv OPENASOCIA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma                
                Campos = new List<MODCamposArchivo>()
            };
            reporte.campos.ForEach(x =>
            {
                OpenAsociaMan.Campos.Add(new MODCamposArchivo { IdCampo = x.Id, Orden = x.Ordinal });
            });
            reporte = estructuras.FirstOrDefault(x => x.Nombre == "EXTR_XM_DIARIO_NF");
            MODArchivo EpsaVeri = new MODArchivo()
            {
                IdReporte = reporte.Id,
                Nombre = "XM_DIARIO_EPSA_VERIFICACION_",
                Descripcion = "Genera archivo de verificacion Epsa",
                IdTipoArchivo = 1, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
                QuitarCabesera = true, //
                Campos = new List<MODCamposArchivo> { 
                    new MODCamposArchivo{ 
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="CODIGO_EVENTO").Id,
                        Orden = 0
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="FECHA_INICIAL").Id,
                        Orden = 1
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="FECHA_FINAL").Id,
                        Orden = 2
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="TIPO_ELEMENTO").Id,
                        Orden = 3
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="COD_CAUSA").Id,
                        Orden = 4
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="EVENTO_CONTINUA").Id,
                        Orden = 5
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="EVENTO_EXCL_ZNI").Id,
                        Orden = 6
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="AFECTA_CONEXION_GEN").Id,
                        Orden = 7
                    },
                    new MODCamposArchivo{
                        IdCampo = reporte.campos.FirstOrDefault(x=>x.Nombre=="USERPUB").Id,
                        Orden = 8
                    }
                }
            };            

            var archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            archivoNegocio.CrearArchivo(OpenAsociaInt);
            archivoNegocio.CrearArchivo(OpenAsociaMan);
            archivoNegocio.CrearArchivo(EpsaVeri);
            EpsaVeri.Nombre = "XM_DIARIO_CETSA_VERIFICACION_";
            EpsaVeri.Descripcion = "Genera archivo de verificacion CETSA";
            archivoNegocio.CrearArchivo(EpsaVeri);
        }
        public static void GenerarReporteDiario()
        {
            Inicializar();
            //CrearEstructurasDiario();
            //CrearArchivosLACDiario();
            CrearReporteDiario();
        }
    }
}