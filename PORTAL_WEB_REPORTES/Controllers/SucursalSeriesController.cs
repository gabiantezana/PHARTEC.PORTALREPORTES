using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Casuarinas.Helpers;
using MSS_REPORTES_TEST.services.ventas.ventasServices;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class SucursalSeriesController : Controller
    {
        public ActionResult Index()
        {
            var resultado = new SucursalSeries().GetList().Where(x=> x.Descripcion !="Todas").OrderBy(x => x.Sucursal?.descripcion).ThenBy(x => x.Serie);
            ViewBag._SucursalId = new SelectList(new PhartecContext().Sucursal.ToList(), "Id", "descripcion", null);
            return View(resultado);
        }

        [HttpPost]
        public JsonResult Save(int? id, int sucursalId, string serie, string descripcion)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "SucursalSeries/Index",
                error = "",
                error2 = ""
            };

            var model = new SucursalSeries();
            if (id != null)
                model.Id = (int)id;

            model.SucursalId = sucursalId;
            model.Serie = serie;
            model.Descripcion = descripcion;


            bool validacion = model.IsValid();
            if (validacion)
            {
                ModelState.AddModelError("", "Ya existe una serie asignada a la misma sucursal");
                respuesta.respuesta = false;
                respuesta.error = "Ya existe una serie asignada a la misma sucursal";
            }
            else
            {
                var result = model.Save();

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
        public ActionResult Delete(int id)
        {
            new Sucursal().Eliminar(id);
            return Redirect("~/Sucursal");
        }

        public bool SucursalContainsSerie(string sucursalName, string serie)
        {
            return GetAuthorizedSeries(sucursalName).Contains(serie);
        }

        public List<string> GetAuthorizedSeries(string sucursalName)
        {
            var authorizedSeries = new PhartecContext().SucursalSeries.Include("Sucursal").Where(x => x.Sucursal.descripcion.Equals(sucursalName)).Select(x => x.Serie).ToList();
            return authorizedSeries;
        }

    }
}
