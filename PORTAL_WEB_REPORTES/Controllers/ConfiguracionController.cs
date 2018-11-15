using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class ConfiguracionController : Controller
    {
        private Configuracion mConfiguracion = new Configuracion();

        public ActionResult Index()
        {
            var exist = mConfiguracion.obtener();

            if (TempData[Constantes.TEMPDATA_MESSAGE] != null)
                ViewData[Constantes.VIEWDATA_ALERT] = Constantes.SUCCESS_MESSAGE;

            return View(exist != null ? exist : mConfiguracion);
        }

        [HttpPost]
        public JsonResult Guardar(Configuracion model)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "configuracion/index",
                error = "",
                error2 = ""
            };

            var val = validate(model);

            if (!string.IsNullOrEmpty(val))
            {
                ModelState.AddModelError("GeneralError", "DataValidations");
                respuesta.respuesta = false;
                respuesta.error = val;
            }

            if (ModelState.IsValid)
            {
                var dbResult = model.Guardar();

                if (string.IsNullOrEmpty(dbResult))
                    TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error = dbResult;
                }
            }

            return Json(respuesta);
        }


        private string validate(Configuracion model)
        {
            var res = String.Empty;

            try
            {
                if (string.IsNullOrEmpty(model.nombre_empresa))
                    res = "Debe ingresar la descripción de la empresa.";
                else if (model.enviar_correos)
                {
                    if (string.IsNullOrEmpty(model.servidor_correo))
                        res = "Debe ingresar el servidor (host) de correos.";
                    else if (model.puerto <= 0)
                        res = "Debe ingresar el puerto del servidor de correos.";
                    else if (string.IsNullOrEmpty(model.usuario))
                        res = "Debe ingresar un usuario.";
                    else if (string.IsNullOrEmpty(model.password))
                        res = "Debe ingresar un password.";
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }
    }
}
