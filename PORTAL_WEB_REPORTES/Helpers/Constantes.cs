using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Casuarinas.Helpers
{
    public class Constantes
    {
        //Constantes de sesión
        public const string SESSION_USUARIO = "UsuarioLogueado";
        public const string SESSION_FROM_PENDENTS = "FromPendents";
        public const string SESSION_NUMBER_MESSAGES = "number_messages";
        public const string CONFIGURACION = "Configuracion";
        public const string ACCESOS = "Accesos";

        public const string SUCCESS_MESSAGE = "success";
        public const string VIEWDATA_ALERT = "alert";
        public const string VIEWDATA_ALERT_APPROVE = "alertApprov";
        public const string VIEWDATA_ALERT_DENIED = "alertDenied";
        public const string VIEWDATA_ALERT_EMAIL = "alertEmail";
        public const string VIEWDATA_ALERT_EMAIL_ERROR = "alertEmailError";
        public const string VIEWDATA_REDIRECT_TO_PENDENTS = "VRedirectToPendents";
        public const string TEMPDATA_MESSAGE = "msg";
        public const string TEMPDATA_MESSAGE_EMAIL = "msgEmail";
        public const string TEMPDATA_MESSAGE_EMAIL_ERROR = "msgErrorEmail";
        public const string TEMPDATA_MESSAGE_APPROVE = "msgAprv";
        public const string TEMPDATA_MESSAGE_DENIED = "msgDenied";
        public const string TEMPDATA_REDIRECT_TO_PENDENTS = "RedirectToPendents";
        
        public const string SAVE_BUTTON_SHOW = "BT_SAVE";
        public const string CANCEL_BUTTON_SHOW = "BT_CANCEL";
        public const string NEW_BUTTON_SHOW = "BT_NEW";
        public const string BACK_BUTTON_SHOW = "BT_BACK";

        public const string ENABLED = "YES";
        public const string DISABLED = "NO";

        public const string SOLICITUD_SESSION_DETAIL = "DataDetalleSolicitud";

        public const string EMAIL_NUEVO_REGISTRO = "1";
        public const string EMAIL_SOLICITUD_PENDIENTE = "2";
        public const string EMAIL_SOLICITUD_ACTUALIZADA = "3";
        public const string EMAIL_RECUPERAR_PASSWORD = "4";

    }
}