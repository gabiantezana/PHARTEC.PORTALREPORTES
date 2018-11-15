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
    public class MenuController : Controller
    {
        private Menu mMenu = new Menu();

        public ActionResult Index(string id= null)
        {
            TempData["clicked"] = id;
            return View();
        }

        public JsonResult GetItemsMenu(JqGrid jq)
        {
            return Json(mMenu.listar(jq), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GuardarItemMenu([Bind(Exclude = "id")]Menu model)
        {
            string r = null;

            try
            {
                if (ModelState.IsValid)
                    model.Guardar();
                else
                    r = "Validación de datos incorrecto";
            }
            catch (Exception e)
            {
                r = e.Message;
            }

            return r;
        }

        [HttpPost]
        public string ActualizarItemMenu(Menu model)
        {
            string r = null;

            try
            {
                if (ModelState.IsValid)
                    model.Guardar();
                else
                    r = "Validación de datos incorrecto";
            }
            catch (Exception e)
            {
                r = e.Message;
            }

            return r;
        }

        [HttpPost]
        public string EliminarItemMenu(Menu model)
        {
            string r = null;

            try
            {
                if (ModelState.IsValid)
                    model.Eliminar();
                else
                    r = "Validación de datos incorrecto";
            }
            catch (Exception e)
            {
                r = e.Message;
            }

            return r;
        }
    }
}
