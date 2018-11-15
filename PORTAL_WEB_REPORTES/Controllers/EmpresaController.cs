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
    public class EmpresaController : Controller
    {
        private Empresa mEmpresa = new Empresa();
        private Estado mEstado = new Estado();
        private MenuRol mPermisos = new MenuRol();

        public ActionResult Index(int? EmpBCod = null, string EmpBDesNom = null, string EmpBest = null)
        {
            var myList = mEmpresa.listar();

            var listEstado = mEstado.listar().Select(x => new SelectListItem{
                Text = x.descripcion,
                Value = x.estado
            });

            if (EmpBCod != null)
                myList = myList.FindAll(e => e.id == (int)EmpBCod);
            else if (EmpBCod == null && !string.IsNullOrEmpty(EmpBDesNom) && string.IsNullOrEmpty(EmpBest))
                myList = myList.FindAll(e => e.descripcion.ToUpper().Contains(EmpBDesNom.ToUpper()) || e.db_name.ToUpper().Contains(EmpBDesNom.ToUpper()));
            else if (EmpBCod == null && string.IsNullOrEmpty(EmpBDesNom) && !string.IsNullOrEmpty(EmpBest))
                myList = myList.FindAll(e => e.estado.Equals(EmpBest)).ToList();
            else if (EmpBCod == null && !string.IsNullOrEmpty(EmpBDesNom) && !string.IsNullOrEmpty(EmpBest))
                myList = myList.FindAll(e => e.estado.Equals(EmpBest) && (e.descripcion.ToUpper().Contains(EmpBDesNom.ToUpper()) || e.db_name.ToUpper().Contains(EmpBDesNom.ToUpper())));

            ViewBag.EmpBest = listEstado;
            return View(myList);
        }

        [HttpPost]
        public JsonResult Eliminar(int id)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "empresa/index",
                error = "",
                error2 = ""
            };

            mEmpresa.id = id;
            var result = mEmpresa.Eliminar();

            if (string.IsNullOrEmpty(result))
            {
                TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = result;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult Guardar(string EmpresaDes, string EmpresaDbn, string EmpresaDbu, string EmpresaDbp, string EmpresaEst, int? EmpresaId = null)
        {
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "empresa/index",
                error = "",
                error2 = ""
            };

            if (EmpresaId != null)
                mEmpresa.id = (int) EmpresaId;
            mEmpresa.descripcion = EmpresaDes;
            mEmpresa.db_name = EmpresaDbn;
            mEmpresa.usuario = EmpresaDbu;
            mEmpresa.password = EmpresaDbp;
            mEmpresa.estado = EmpresaEst;
            var result = mEmpresa.Guardar();

            if (string.IsNullOrEmpty(result))
            {
                TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = result;
            }

            return Json(respuesta);
        }
    }
}
