using Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Data;
namespace Casuarinas.Controllers
{
    public class OrdenCompraController : Controller
    {
        //
        // GET: /OrdenCompra/

        //Listado
        public ActionResult Index()
        {
            /*
            Uri myUri = new Uri("http://192.168.1.52:8000/WS/services/items/oGetNewItem.xsodata");
            HanaOData.oGetNewItem myService = new HanaOData.oGetNewItem(myUri);

            var result = from i in myService.getListNewItems
                         select i;

            var filter = result.Take(2).ToList();
            ViewBag.resultado = result;
            return View(result);

    */
            return View();
        }


        public void oDataList()
        {
            /*
            Uri myUri = new Uri("http://192.168.1.52:8000/WS/services/purchaseorders/slopesPurchaseOrders.xsodata");
            Servicio.slopesPurchaseOrders miservicio = new Servicio.slopesPurchaseOrders(myUri);

            var result = from i in miservicio.listPurchaseOrders select i;

            ArrayList ordenes = new ArrayList();
            OrdenCompra o = new OrdenCompra();

            //foreach (var item in result)
            //{
            //    item.BatchNum = o.BatchNumms;
            //    item.CardCode = o.CardCodems;
            //    item.CardName = o.CardNamems;
            //    item.CodeBars = o.CodeBarsItemms;
            //    item.CodeBarsItem = o.CodeBarsItemms;
            //    item.DocEntry = o.DocEntryms;
            //    item.Dscription = o.Dscriptionms;
            //    item.familia = o.familiams;
            //    item.Height1 = o.Height1ms;
            //    item.ItemCode = o.ItemCodems;
            //    item.Length1 = o.Length1ms;
            //    item.LineNum = o.LineNumms;
            //    item.ManBtchNum = o.ManBtchNumms;
            //    item.ManSerNum = o.ManSerNumms;
            //    item.Quantity = o.Quantityms;
            //    item.quantityFormat = o.quantityFormatms;
            //    item.SerNum = o.SerNumms;
            //    item.subfamilia = o.subfamiliams;
            //    item.TaxDate = o.TaxDatems;
            //    item.U_MSS_CODREF = o.U_MSS_CODREFms;
            //    item.U_MSS_FAMILIA = o.U_MSS_FAMILIAms;
            //    item.U_MSS_SUBFAMILIA = o.U_MSS_SUBFAMILIAms;
            //    item.U_MSS_TIPC = o.U_MSS_TIPCms;
            //    item.unitMsr = o.unitMsrms;
            //    item.Weight1 = o.Weight1ms;
            //    item.Width1 = o.Width1ms;

            //    ordenes.Add(o);
            //}
            */

        }
    }
}
