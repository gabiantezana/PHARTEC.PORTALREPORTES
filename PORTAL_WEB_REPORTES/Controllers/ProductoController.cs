using Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Casuarinas.Controllers
{
    public class ProductoController : Controller
    {
        //
        // GET: /Producto/
        private Producto producto = new Producto();
        public ActionResult Index()
        {
            return View(producto.listar());
        }

        public void oDataList()
        {
            /*
            Uri myUri = new Uri("http://192.168.1.52:8000/WS/services/items/oGetNewItem.xsodata");
            HanaOData.oGetNewItem myService = new HanaOData.oGetNewItem(myUri);

            var result = from i in myService.getListNewItems
                         select i;

            
            foreach (var item in result)
            {
                
            }

         //   var filter = result.Take(2).ToList();
         */
        }

  

        public JsonResult Obtenerdatos()
        {
            IRestResponse result;
            RestClient cliente = new RestClient("http://192.168.1.52:8000/WS/services/items/getItems.xsjs");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "text/xml");
            result = cliente.Execute(request);
            

            return Json(result.Content);
        }

    }
}
