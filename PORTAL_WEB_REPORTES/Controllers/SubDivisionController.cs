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
    public class SubDivisionController : Controller
    {
        //
        // GET: /SubDivision/
        private SubDivision mSubdivision = new SubDivision();
        public ActionResult Index()
        {
            var resultado = mSubdivision.Listar();
            return View(resultado);
        }

        //Método: Guardar Subdivisión

        [HttpPost]
        public JsonResult Guardar(string SubDes, string SubCodSap, int? SubId = null) {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "subdivision/index",
                error = "",
                error2 = ""
            };
            if (SubId != null)
                mSubdivision.Id = (int)SubId;
            mSubdivision.descripcion = SubDes;
            mSubdivision.codigoSap = SubCodSap;

            bool validacion = mSubdivision.ValidateCodSap();
            if (validacion)
            {
                ModelState.AddModelError("", "Código Sap ya existe");
                respuesta.respuesta = false;
                respuesta.error = "El código sap especificado ya ha sido asignado";

            }
            if (!validacion) {
                var result = mSubdivision.Guardar();
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

        //Método: Eliminar Subdivisión
        public ActionResult Eliminar(int id)
        {
            mSubdivision.Eliminar(id);
            return Redirect("~/Subdivision");
        }
    }
}
