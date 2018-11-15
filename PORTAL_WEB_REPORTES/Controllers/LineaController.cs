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
    public class LineaController : Controller
    {
        //
        // GET: /Linea/


        private Linea mLinea = new Linea();
        public ActionResult Index()
        {
            var resultado = mLinea.Listar();
            return View(resultado);
        }

        [HttpPost]
        public JsonResult Guardar(string LineaDes, string LineaCodSap, int? LineaId = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "linea/index",
                error = "",
                error2 = ""
            };
            if (LineaId != null)

                mLinea.Id = (int)LineaId;
            mLinea.descripcion = LineaDes;
            mLinea.codigoSap = LineaCodSap;

            bool validacion = mLinea.ValidateCodSap();
            if (validacion)
            {
                ModelState.AddModelError("", "Código Sap ya existe");
                respuesta.respuesta = false;
                respuesta.error = "El código sap especificado ya ha sido asignado";

            }
            if (!validacion)
            {
                var result = mLinea.Guardar();
                if (string.IsNullOrEmpty(result))
                {
                    TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error2 = result;
                }
            }

            return Json(respuesta);
        }

        //Método: Eliminar Línea
        public ActionResult Eliminar(int id)
        {
            mLinea.Eliminar(id);
            return Redirect("~/Linea");
        }



    }
}
