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
    public class ArticuloController : Controller
    {
        private Articulo mArticulo = new Articulo();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getAllItems(string prefix, string type)
        {
            var articulos = mArticulo.listar();

            var ItemNames = (from a in articulos
                             where type.Equals("D")? a.descripcion.ToUpper().Contains(prefix.ToUpper()) :
                                        a.codigo_sap.ToUpper().Contains(prefix.ToUpper())
                             select new { a.codigo_sap, a.descripcion });

            return Json(ItemNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getArticuloXEmpresa(int idEmpresa)
        {
            var list = mArticulo.listarXEmpresa(idEmpresa).Select(x => new SelectListItem
            {
                Text = x.descripcion,
                Value = x.codigo_sap
            });

            return Json(list);
        }

        public int getIdArticulo(string codigoSap)
        {
            var id = 0;

            var res = mArticulo.obtenerId(codigoSap);
            if (res >= 0)
                id = res;

            return id;
        }
    }
}
