using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class DivisionController : Controller
    {
        //
        // GET: /Division/

        private Division mDivision = new Division();

        public ActionResult Index()
        {
            var resultado = mDivision.Listar();
            return View(resultado);
        }

        //Método: Guardar División

        [HttpPost]
        public JsonResult Guardar(string DivisionDes, string DivisionCodSap, int? DivisionId = null) {

            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "division/index",
                error = "",
                error2 = ""
            };

            if (DivisionId != null)
                mDivision.Id = (int)DivisionId;
            mDivision.descripcion = DivisionDes;
            mDivision.codigoSap = DivisionCodSap;

            bool validacion = mDivision.ValidateCodSap();
            if (validacion)
            {
                ModelState.AddModelError("", "Código Sap ya existe");
                respuesta.respuesta = false;
                respuesta.error = "El código sap especificado ya ha sido asignado";

            }

            if (!validacion)
            {
                var result = mDivision.Guardar();
                if (string.IsNullOrEmpty(result))
                {
                    TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error = result;
                }
            }

            return Json(respuesta);

        }


        //Método: Eliminar División
        public ActionResult Eliminar(int id) {
            mDivision.Eliminar(id);
            return Redirect("~/Division");
        }

    }
}
