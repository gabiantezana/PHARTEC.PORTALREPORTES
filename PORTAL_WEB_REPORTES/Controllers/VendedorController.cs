using Casuarinas.Helpers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class VendedorController : Controller
    {
        //
        // GET: /Vendedor/

        public ActionResult Index(string descripcion)
        {
            //Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
            //if (objCurrentUser == null)
            //{
            //    SessionHelper.DestroyUserSession();
            //    return Redirect("~/home/index");
            //}
            //var sucList = objCurrentUser.Sucursales;
           
            //var listSucursales = sucList.Select(x => new SelectListItem
            //{
            //    Text = x.descripcion,
            //    Value = x.descripcion
                
            //});

            //ViewBag.descripcion = listSucursales;

            //var divList = objCurrentUser.Divisiones;
            //ViewBag.divisionx = divList.Select(x => new SelectListItem
            //{
            //    Text = x.descripcion,
            //    Value = x.codigoSap,
            //    Selected = true

            //});

            //var subList = objCurrentUser.Subdivisiones;
            //ViewBag.subdivisionx = subList.Select(x => new SelectListItem
            //{
            //    Text=x.descripcion,
            //    Value=x.codigoSap,
            //    Selected=true
            //});

            //var linList = objCurrentUser.Lineas;
            //ViewBag.lineax = linList.Select(x => new SelectListItem
            //{
            //    Text = x.descripcion,
            //    Value = x.codigoSap,
            //    Selected = true
            //});

            //Uri myUri = new Uri("http://192.168.1.52:8000/MSS_REPORTES/services/ventas/ventasServices.xsodata/");
            //ventasServices myService = new ventasServices(myUri);

            //if (string.IsNullOrEmpty(descripcion))
            //{
            //    var result = from i in myService.ventasClientes
            //                 group i by new {i.CardCode,i.CardName,i.Sucursal}  into x
            //                 select x;
            //    ViewBag.resultado = result;
            //}
            //else
            //{
            //    var result = from i in myService.ventasClientes
            //                 where i.Sucursal.Equals(descripcion)
            //                 select i;
            //    ViewBag.resultado = result;
            //}
            return View();
            


        }

    }
}
