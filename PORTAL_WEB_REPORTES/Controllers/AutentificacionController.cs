using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    public class AutentificacionController : Controller
    {
        //
        // GET: /Autentificacion/
        private Usuario usuario = new Usuario();
        private Configuracion configuracion = new Configuracion();
        private MenuRol mAccesos = new MenuRol();

        public ActionResult Index()
        {
            return View();
        }

        // METODO - SIN VISTA
        public JsonResult IniciarSesion(string usuarioWeb, string password)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "home/index",
                error = ""
            };

            var query = usuario.obtener(usuarioWeb);
            if (query != null)
            {
                var hashCode = query.VCode;
                var encodingPasswordString = PasswordHelper.EncodePassword(password, hashCode);

                var validaUsuario = usuario.validar(usuarioWeb, encodingPasswordString);
                if (string.IsNullOrEmpty(validaUsuario))
                {
                    var objUsuario = usuario.obtener(usuarioWeb);
                    var objConfiguracion = configuracion.obtener();
                    var objAccesos = mAccesos.ObtenerXRol(objUsuario.Rol_id);
                    Session[Constantes.SESSION_USUARIO] = objUsuario;
                    Session[Constantes.CONFIGURACION] = objConfiguracion;
                    Session[Constantes.ACCESOS] = objAccesos;
                    SessionHelper.AddUserToSession(objUsuario.id.ToString());
                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error = validaUsuario;
                    respuesta.redirect = "autentificacion/index";
                    ModelState.AddModelError("errorLogin", validaUsuario.ToString());
                }
            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = "El usuario no existe";
                respuesta.redirect = "autentificacion/index";
                ModelState.AddModelError("errorLogin", "User no exists");
            }

            if (ModelState.IsValid)
            {
                TempData[Constantes.TEMPDATA_MESSAGE] = true;
                respuesta.redirect = "home/index";
            }

            return Json(respuesta);
        }

        public JsonResult forgotPassword(string correo, string user)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "/home/index",
                error = ""
            };

            var query = usuario.getIDX(correo, user);
            if (query != null)
            {
                string newPassword = ForgotPasswordHelper.Generate(6);
                var current_user = usuario.obtener((int)query);
                var encodingPasswordString = PasswordHelper.EncodePassword(newPassword, current_user.VCode);
                usuario.ActualizarPassword((int)query, encodingPasswordString);

                EmailModel email = new EmailModel();
                email.ToEmail = correo;
                email.Subject = "Recuperación de clave de acceso";
                email.userName = user;
                email.extraValue = newPassword;
                var sendEmail = new EmailHelper().sendEmail(configuracion.obtener(), email, Constantes.EMAIL_RECUPERAR_PASSWORD);
            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = "Los datos ingresados no son correctos";
            }

            return Json(respuesta);
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            SessionHelper.DestroyUserSession();
            return Redirect("~/home/index");
        }

    }
}
