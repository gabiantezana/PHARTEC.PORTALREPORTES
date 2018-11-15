using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Casuarinas.Helpers;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class SucursalController : Controller
    {
        //
        // GET: /Sucursal/

        private Sucursal mSucursal = new Sucursal();

        public ActionResult Index()
        {
            var resultado = mSucursal.listar();
            return View(resultado);
        }


        //Método registrar Sucursal
        [HttpPost]
        public JsonResult Guardar(Sucursal model, string SucursalDes, string SucursalCodSap, int? SucursalId = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "sucursal/index",
                error = "",
                error2 = ""
            };
            if (SucursalId != null)
                model.Id = (int)SucursalId;
            model.descripcion = SucursalDes;
            model.codigoSap = SucursalCodSap;


            bool validacion = model.ValidateCodSap();
            if (validacion)
            {
                ModelState.AddModelError("", "Código Sap ya existe");
                respuesta.respuesta = false;
                respuesta.error = "El código sap especificado ya ha sido asignado";

            }

            if (!validacion)
            {

                var result = model.Guardar();

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


        //Método eliminar Sucursal
        public ActionResult Eliminar(int id)
        {
            mSucursal.Eliminar(id);
            return Redirect("~/Sucursal");
        }
    }
}
