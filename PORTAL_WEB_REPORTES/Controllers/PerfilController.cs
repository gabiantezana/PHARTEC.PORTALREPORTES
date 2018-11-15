using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class PerfilController : Controller
    {
        private Usuario usuario = new Usuario();
        private Rol rol = new Rol();
        

        // GET: /Perfil/
        public ActionResult Index()
        {
            usuario = (Usuario) Session[Constantes.SESSION_USUARIO];
            ViewBag.Rol_Id = new SelectList(rol.listar(), "id", "descripcion", usuario != null ? usuario.Rol_id : 0);

            return View(usuario);
        }

        [HttpPost]
        public JsonResult Actualizar(Usuario model)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "perfil/index",
                error = "",
                error2 = ""
            };

            if (model == null)
            {
                ModelState.AddModelError("usuario", "Valores de usuario no válidos.");
                respuesta.respuesta = false;
                respuesta.error = "Valores de usuario no válidos";
            }

            var userExists = model.validateUserAccount();
            if (userExists)
            {
                ModelState.AddModelError("", "Usuario existente");
                respuesta.respuesta = false;
                respuesta.error = "El usuario especificado ya existe.";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.Actualizar();
                    TempData[Casuarinas.Helpers.Constantes.TEMPDATA_MESSAGE] = "Success";
                }
                catch (DbEntityValidationException e)
                {
                    respuesta.respuesta = false;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        respuesta.error = "Entity of type " + eve.Entry.Entity.GetType().Name
                            + " in state " + eve.Entry.State + " has the following validation errors:";
                        
                        foreach (var ve in eve.ValidationErrors)
                            respuesta.error += " - Property: "+ve.PropertyName+", Error:  "+ve.ErrorMessage;
                    }
                }
                catch (Exception ex)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Error Actualizando, " + ex.Message;
                }


                var objUsuario = usuario.obtener(model.id);
                Session[Constantes.SESSION_USUARIO] = objUsuario;
                respuesta.redirect = "perfil/index";
            }
            else
                respuesta.respuesta = false;

            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult CambiarPassword(string actual, string nueva)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                error = "",
            };

            var current_user = usuario.obtener(SessionHelper.GetUser());
            var hashCode = current_user.VCode;
            var encodingPasswordString = PasswordHelper.EncodePassword(actual, hashCode);

            var val = usuario.validar(current_user.cuentaWeb, encodingPasswordString);
            if (string.IsNullOrEmpty(val))
            {
                //Si es válido proceder a actualizar la contraseña
                // Encriptar contraseña nueva
                string newPasswordEncoding = PasswordHelper.EncodePassword(nueva, hashCode);
                // Registrar
                usuario.ActualizarPassword(current_user.id, newPasswordEncoding);
            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = val;
            }

            return Json(respuesta);
        }
    }
}
