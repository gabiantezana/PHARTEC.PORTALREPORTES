using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class PendientesController : Controller
    {
        private Solicitud mSolicitud = new Solicitud();
        private Usuario mUsuario = new Usuario();
        private Notificacion mWebNotificacion = new Notificacion();
        private MenuRol mPermisos = new MenuRol();
        private Configuracion objConfiguracion;

        // VIEW - Index lista principal
        public ActionResult Index(string from = null, string to = null, int user = 0)
        {
            List<SolicitudListModel> resultado = mSolicitud.listarXUsuario(SessionHelper.GetUser());

            var listUsers = mUsuario.getForCombo().Select(x => new SelectListItem
            {
                Text = x.nombre,
                Value = x.id.ToString()
            });

            if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && user <= 0)
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && user <= 0)
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    && u.FechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && user > 0)
                resultado = resultado.FindAll(u => u.UsuarioId == user &&
                    (u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                     && u.FechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture)));
            else if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && user > 0)
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture) && u.UsuarioId == user);
            else if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && user > 0)
                resultado = resultado.FindAll(u => u.UsuarioId == user);

            HttpContext.Session.Remove(Constantes.SOLICITUD_SESSION_DETAIL);
            HttpContext.Session.Remove(Constantes.SESSION_FROM_PENDENTS);

            ViewBag.user = listUsers;
            return View(resultado);
        }

        // VIEW - Detalles
        public ActionResult Details(int id)
        {
            Session[Constantes.SESSION_FROM_PENDENTS] = true;
            return RedirectToAction("Details", "Solicitud", new { id = id});
        }

        // VIEW - Editar
        public ActionResult Form(int id)
        {
            Session[Constantes.SESSION_FROM_PENDENTS] = true;
            return RedirectToAction("Form", "Solicitud", new { id = id });
        }

        [HttpPost]
        public JsonResult AprobarSolicitud(int[] ids = null, string comentario = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "pendientes/index",
                error = "",
                error2 = ""
            };

            if (ids != null)
            {
                int counter = 0;
                foreach (var uid in ids)
                {
                    mSolicitud.id = uid;
                    if (comentario != null)
                        mSolicitud.comentarios = comentario;

                    var res = mSolicitud.Aprobar(SessionHelper.GetUser());
                    if (string.IsNullOrEmpty(res))
                    {
                        counter++;
                        var lastLevel = mSolicitud.obtenerEstadoActivo(uid);
                        if (lastLevel != null)
                            notificarUsuario(uid, mUsuario.obtener(lastLevel.Usuario), "SOLICITUD PENDIENTE", Constantes.EMAIL_SOLICITUD_PENDIENTE);

                        notificarUsuario(uid, mUsuario.obtener(mSolicitud.obtener(uid).Usuario_id), "ACTUALIZACIÓN SOLICITUD (APROBACIÓN)", Constantes.EMAIL_SOLICITUD_ACTUALIZADA);
                        mWebNotificacion.updateNumPendMessages(SessionHelper.GetUser(), mSolicitud, "success");
                    }
                    else
                    {
                        respuesta.respuesta = false;
                        respuesta.error += "Solicitud " + uid + ", Error "+ res +".";
                    }
                }

                if (counter > 0)
                {
                    TempData[Constantes.TEMPDATA_MESSAGE_APPROVE] = "Solicitudes aprobadas con éxito.";
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult RechazarSolicitud(int[] ids = null, string comentario = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "pendientes/index",
                error = "",
                error2 = ""
            };

            if (ids != null)
            {
                int counter = 0;
                foreach (var uid in ids)
                {
                    mSolicitud.id = uid;
                    if (comentario != null)
                        mSolicitud.comentarios = comentario;

                    var res = mSolicitud.Rechazar(SessionHelper.GetUser());
                    if (string.IsNullOrEmpty(res))
                    {
                        counter++;
                        notificarUsuario(uid, mUsuario.obtener(mSolicitud.obtener(uid).Usuario_id), "ACTUALIZACIÓN SOLICITUD (RECHAZO)", Constantes.EMAIL_SOLICITUD_ACTUALIZADA);
                        mWebNotificacion.updateNumPendMessages(SessionHelper.GetUser(), mSolicitud, "block alert-danger");
                    }
                    else
                    {
                        respuesta.respuesta = false;
                        respuesta.error += "Solicitud " + uid + ", Error " + res + ".";
                    }
                }

                if (counter > 0)
                {
                    TempData[Constantes.TEMPDATA_MESSAGE_DENIED] = "Solicitudes rechazadas con éxito.";
                }
            }

            return Json(respuesta);
        }

        //Util
        private void notificarUsuario(int solID, Usuario user, string subject, string type)
        {
            objConfiguracion = (Configuracion)Session[Constantes.CONFIGURACION];

            EmailModel email = new EmailModel();
            email.ToEmail = user.correo;
            email.Subject = "MSS SEIDOR WEB REQ - "+ subject +" N° " + solID;
            email.userName = user.nombre;
            email.solID = solID;
            var sendEmail = new EmailHelper().sendEmail(objConfiguracion, email, type);
        }

    }
}
