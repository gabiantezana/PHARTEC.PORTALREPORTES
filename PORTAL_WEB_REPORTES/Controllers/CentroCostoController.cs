using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class CentroCostoController : Controller
    {
        //
        // GET: /CentroCosto/
        private CentroCosto ccModel = new CentroCosto();
        private Empresa empresa = new Empresa();
        private Estado modelEstado = new Estado();
        private Usuario usuario = new Usuario();
        private CentroCostoNivel niveles = new CentroCostoNivel();
        private MenuRol mPermisos = new MenuRol();

        //Listado
        public ActionResult Index(string search = null)
        {
            List<CentroCosto> resultado = ccModel.listar();

            if (!string.IsNullOrEmpty(search))
            {
                resultado = resultado.FindAll(u => u.descripcion.ToUpper().Contains(search.Trim().ToUpper()) ||
                                                   u.Empresa.descripcion.ToUpper().Contains(search.Trim().ToUpper()));
            }

            HttpContext.Session.Remove("DataNiveles");
            return View(resultado);
        }

        // VIEW - Inserción y edición
        public ActionResult Form(int id = 0)
        {
            var objCentroCosto = id > 0 ? ccModel.obtener(id) : ccModel;

            var list = modelEstado.listar().Select(x => new SelectListItem
            {
                Text = x.descripcion,
                Value = x.estado,
                Selected = (x.estado == objCentroCosto.estado)
            });

            var levels = objCentroCosto.CentroCostoNivel.ToList();
            if (HttpContext.Session["DataNiveles"] != null && levels != null)
                levels = fillListDataNivel(true, levels);
            else if (HttpContext.Session["DataNiveles"] == null && levels != null)
                levels = fillListDataNivel(false, levels);

            ViewBag.Niveles = levels;
            ViewBag.Usuarios = usuario.getForCombo();
            ViewBag.Estado = list;
            return View(objCentroCosto != null ? objCentroCosto : ccModel);
        }


        [HttpPost]
        public JsonResult Guardar(CentroCosto model)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "centrocosto/form/" + model.id,
                error = "",
                error2 = ""
            };

            List<CentroCostoNivel> objList = (List<CentroCostoNivel>)HttpContext.Session["DataNiveles"];

            if (objList != null)
            {
                model.CentroCostoNivel = objList;

                foreach (var item in model.CentroCostoNivel)
                    item.CentroCosto_id = model.id;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    respuesta.error = model.Guardar();

                    if (String.IsNullOrEmpty(respuesta.error))
                    {
                        respuesta.redirect = "centrocosto/form/" + model.id;
                        TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                        HttpContext.Session["DataNiveles"] = null;
                        HttpContext.Session.Remove("DataNiveles");
                    }
                    else
                        respuesta.respuesta = false;
                }
                else
                {
                    respuesta.respuesta = false;
                }
            }
            catch (Exception e)
            {
                respuesta.respuesta = false;
                respuesta.error = e.Message;
            }

            return Json(respuesta);
        }

        public PartialViewResult DataNiveles(string descripcion = null, int userId = 0, string userName = null, string[] prioridades = null, string deleteId = null)
        {
            List<CentroCostoNivel> objList = (List<CentroCostoNivel>) HttpContext.Session["DataNiveles"];

            if(descripcion != null && userId != 0 && userName != null){
                if (objList == null)
                    objList = new List<CentroCostoNivel>();

                CentroCostoNivel objNew = new CentroCostoNivel();
                objNew.temporal_id = Guid.NewGuid().ToString("N");
                objNew.descripcion = descripcion;
                objNew.prioridad = objList.Count + 1;
                objNew.Usuario_id = userId;
                objNew.Usuario_Nombre = userName;

                objList.Add(objNew);
            }
            else if (prioridades != null)
            {
                if (objList != null)
                {
                    foreach (var item in objList)
                    {
                        for (int i = 0; i < prioridades.Count(); i++)
                        {
                            string[] line = prioridades[i].Split('|');

                            if (line.Length > 1)
                            {
                                string id = line.GetValue(0).ToString().Trim();
                                string order = line.GetValue(1).ToString().Trim();

                                if (item.temporal_id.Equals(id.Trim()))
                                    item.prioridad = int.Parse(order);
                            }
                        }
                    }

                    var sortList = objList.OrderBy(n => n.prioridad).ToList();
                    objList.Clear();
                    objList.AddRange(sortList);
                }
            }
            else if (deleteId != null)
            {
                for (int i = objList.Count -1; i >= 0; i--)
                {
                    var item = objList[i];
                    if (item.temporal_id.Equals(deleteId))
                    {
                        objList.RemoveAt(i);
                        break;
                    }
                }

                int newPriority = 1;
                foreach (var item in objList)
                {
                    item.prioridad = newPriority;
                    newPriority++;
                }

            }

            HttpContext.Session["DataNiveles"] = objList;
            ViewBag.Niveles = objList;
            return PartialView();
        }

        //utils
        private List<CentroCostoNivel> fillListDataNivel(bool sessionExists, List<CentroCostoNivel> existentList)
        {
            List<CentroCostoNivel> currentList;

            if (sessionExists)
                currentList = (List<CentroCostoNivel>)HttpContext.Session["DataNiveles"];
            else
                currentList = new List<CentroCostoNivel>();

            foreach (var item in existentList)
            {
                try
                {
                    var founded = currentList.Find(n => n.temporal_id.Equals(item.temporal_id));

                    if (founded == null)
                    {
                        var objNew = new CentroCostoNivel();
                        objNew.temporal_id = item.temporal_id;
                        objNew.descripcion = item.descripcion;
                        objNew.prioridad = item.prioridad;
                        objNew.Usuario_id = item.Usuario_id;
                        objNew.Usuario_Nombre = item.Usuario_Nombre;
                        currentList.Add(objNew);
                    }
                }
                catch (Exception)
                {
                }
            }

            HttpContext.Session["DataNiveles"] = currentList;
            return currentList;
        }

        //public JsonResult getCentroCostoXEmpresa(int idEmpresa)
        //{
        //    //Filtrar la lista por los centros de costo que el usuario tiene asignado
        //    Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
        //    var s = from c in objCurrentUser.CentrosCosto
        //            where c.Empresa_id == idEmpresa
        //            select c;

        //    var listFinal = s.Select(x => new SelectListItem {
        //                                Text = x.descripcion,
        //                                Value = x.id.ToString()
        //                            });

        //    return Json(listFinal);
        //}
    }
}
