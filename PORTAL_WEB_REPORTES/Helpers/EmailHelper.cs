using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Model;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;

namespace Casuarinas.Helpers
{
    public class EmailHelper
    {

        public string sendEmail(Configuracion config, EmailModel email, string TYPE)
        {
            var result = string.Empty;

            try
            {
                using (MailMessage mail = new MailMessage(new MailAddress(config.usuario, "Phartec"), new MailAddress(email.ToEmail)))
                {
                    mail.Subject = email.Subject;
                    mail.Body = TYPE.Equals(Constantes.EMAIL_RECUPERAR_PASSWORD) ? buildBodyForgotPassword(email.userName, email.extraValue) : buildBodyHtml(email.userName, email.solID, TYPE);
                    mail.Priority = MailPriority.High;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        NetworkCredential networkCredential = new NetworkCredential(config.usuario, config.password);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = networkCredential;
                        smtp.Port = config.puerto;
                        smtp.Host = config.servidor_correo;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }


        public string buildBodyHtml(string userName, int solID, string type)
        {
            var body = string.Empty;

            body += "<h1 style=\"color: #5e9ca0;\"Portal Web de Reportes</h1>";
            body += "<h2 style=\"color: #2e6c80;\">Tiene una nueva notificaci&oacute;n:</h2>";

            if (type.Equals(Constantes.EMAIL_NUEVO_REGISTRO))
                body += "<p>El usuario  <b>" + userName + "</b> acaba de registrar una solicitud de requerimiento con identificador  <b>" + solID + "</b>. ";
            else if (type.Equals(Constantes.EMAIL_SOLICITUD_PENDIENTE))
                body += "<p>Tiene una solicitud de requerimiento pendiente por aprobar con identificador  <b>" + solID + "</b>. ";
            else if (type.Equals(Constantes.EMAIL_SOLICITUD_ACTUALIZADA))
                body += "<p>Su solicitud con identificador  <b>" + solID + "</b> tiene una nueva actualización de estado. ";

            body += "Ingrese con su usuario y contrase&ntilde;a para poder validarla. <br />";

            if (type.Equals(Constantes.EMAIL_NUEVO_REGISTRO) || type.Equals(Constantes.EMAIL_SOLICITUD_PENDIENTE))
                body += "Cuando la apruebe o rechaze se le notificar&aacute; a las personas correspondientes.&nbsp;</p>";

            body += "<p>Saludos cordiales.</p>";
            body += "<img style=\"float: right;\" src=\"http://www.phartecperu.com/images/Logo-Phartec.png\"  width =\"180\" />";

            return body;
        }

        public string buildBodyForgotPassword(string userName, string newPassword)
        {
            string body = string.Empty;

            body += "<h1 style=\"color: #5e9ca0;\">Portal Web de Reportes</h1>";
            body += "<h2 style=\"color: #2e6c80;\">Se ha reestablecido su contraseña</h2>";
            body += "<p>Se ha generado una nueva clave de acceso a su cuenta <b>" + userName + "</b>.</br>";
            body += "Le recomendamos cambiarla por razones de seguridad. <br />";
            body += "Su nueva clave de acceso: " + newPassword + " <br />";

            body += "<p>Saludos cordiales.</p>";
            body += "<img style=\"float: right;\" src=\"http://www.phartecperu.com/images/Logo-Phartec.png\"  width=\"180\" />";

            return body;
        }
    }
}