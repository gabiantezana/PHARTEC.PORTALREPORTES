using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Casuarinas.Helpers
{
    public class ConstantesSolicitud
    {

    }

    public class EstadoSolicitud
    {
        public const string INICIAL_REGISTRADA = "R";
        public const string PROCESANDO = "P";
        public const string APROBADA = "A";
        public const string RECHAZADA = "D";
        public const string RegistradaEnSAP = "1";
        public const string OrdenDeCompra = "2";
        public const string IngresoAlmacen = "3";
    }

    public class SaveMessageValidation
    {
        public const string EMPRESA_MODEL_ERROR = "empresa";
        public const string SELECCIONE_EMPRESA = "Debe seleccionar una empresa";

        public const string CENTROCOSTO_MODEL_ERROR = "centroCosto";
        public const string SELECCIONE_CENTROCOSTO = "Debe seleccionar un centro de costo";

        public const string NAPROBACION_MODEL_ERROR = "aprobacion";
        public const string SELECCIONE_NAPROBACION = "El centro de costo no tiene niveles de aprobación asignados";

        public const string DETALLES_MODEL_ERROR = "detalles";
        public const string SELECCIONE_DETALLES = "Debe seleccionar por lo menos un artículo/servicio";
    }
}