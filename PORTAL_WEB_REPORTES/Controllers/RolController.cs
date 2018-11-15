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
    public class RolController : Controller
    {
        private Rol mRol = new Rol();
        private Menu mMenu = new Menu();
        private MenuRol mPermisos = new MenuRol();

        public ActionResult Index()
        {
            if (TempData[Constantes.TEMPDATA_MESSAGE] != null)
                ViewData[Constantes.VIEWDATA_ALERT] = Constantes.SUCCESS_MESSAGE;

            return View(mRol.listar());
        }

        public ActionResult Form(int id=0)
        {
            var objRol = id > 0 ? mRol.obtener(id) : mRol;

            if (TempData[Constantes.TEMPDATA_MESSAGE] != null)
                ViewData[Constantes.VIEWDATA_ALERT] = Constantes.SUCCESS_MESSAGE;

            var listMenu = mMenu.listar();
            if (id == 0)
            {
                foreach (var item in listMenu)
                {
                    mRol.MenuRol.Add(castMenuToRol(item));
                }
            }
            else
            {
                if (objRol != null && objRol.MenuRol.Count == 0)
                {
                    foreach (var item in listMenu)
                    {
                        objRol.MenuRol.Add(castMenuToRol(item));
                    }
                }

                foreach (var item in listMenu)
                {
                    var r = (from mxr in objRol.MenuRol
                            where mxr.Menu_id == item.id
                            select mxr).ToList();
                    if (r.Count == 0)
                        objRol.MenuRol.Add(castMenuToRol(item));

                    foreach (var mxrol in objRol.MenuRol)
                    {
                        if (mxrol.Menu_id == item.id)
                        {
                            mxrol.Menu_descripcion = item.descripcion;
                            mxrol.Menu_orden = (int)item.orden;
                        }
                    }
                }
            }

            return View(id > 0 ? objRol : mRol);
        }

        public ActionResult Details(int id)
        {
            return RedirectToAction("Form", new { id = id});
        }

        [HttpPost]
        public JsonResult Guardar(Rol model, string[] mxrol = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "rol/form/" + model.id,
                error = "",
                error2 = ""
            };

            if (string.IsNullOrEmpty(model.descripcion))
            {
                ModelState.AddModelError("description", "Debe ingresar una descripción");
                respuesta.respuesta = false;
                respuesta.error = "Debe ingresar una descripción";
            }
            else if (mxrol == null)
            {
                ModelState.AddModelError("mxrol", "No se seleccionó ninguna autorización de menú");
                respuesta.respuesta = false;
                respuesta.error = "No se seleccionó ninguna autorización de menú";
            }

            if (ModelState.IsValid)
            {
                bool newObject = model.id <= 0;

                foreach (var row in mxrol)
                {
                    var arrRow = row.Split('|');
                    var item = new MenuRol();
                    item.Menu_id = int.Parse(arrRow[0].ToString().Trim());
                    item.accesa = arrRow[1].ToString().Trim().Equals("1");
                    item.registra = arrRow[2].ToString().Trim().Equals("1");
                    item.modifica = arrRow[3].ToString().Trim().Equals("1");
                    item.consulta = arrRow[4].ToString().Trim().Equals("1");
                    item.elimina = arrRow[5].ToString().Trim().Equals("1");
                    item.imprime = arrRow[6].ToString().Trim().Equals("1");
                    item.exporta = arrRow[7].ToString().Trim().Equals("1");
                    item.estado = "A";
                    item.fecha_registro = DateTime.Now;
                    item.Menu = new Menu { id = item.Menu_id };
                    model.MenuRol.Add(item);
                }

                model.fechaRegistro = DateTime.Now;
                var dbResult = model.Guardar();

                if (string.IsNullOrEmpty(dbResult))
                {
                    TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error = dbResult;
                }

                respuesta.redirect = "rol/index";
            }

            return Json(respuesta);
        }

        private MenuRol castMenuToRol(Menu item)
        {
            MenuRol mxrol = new MenuRol();
            mxrol.Menu_id = item.id;
            mxrol.accesa = false;
            mxrol.registra = false;
            mxrol.modifica = false;
            mxrol.consulta = false;
            mxrol.elimina = false;
            mxrol.imprime = false;
            mxrol.exporta = false;
            mxrol.Menu_descripcion = item.descripcion;
            mxrol.Menu_orden = (int)item.orden;

            return mxrol;
        }
    }
}
