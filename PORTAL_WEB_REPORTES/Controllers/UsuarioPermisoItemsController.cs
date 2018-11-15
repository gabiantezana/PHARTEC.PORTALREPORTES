using Casuarinas.Helpers;
using Model;
using MSS_REPORTES_TEST.services.ventas.ventasServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Casuarinas.Controllers
{
    internal class UsuarioPermisoItemsController : Controller
    {
        public List<AuthorizationItem> GetAuthorizedItems(TipoTablaSAP tipoTablaSAP, int? usuarioId, Uri urlServiceHana)
        {
            var hanaService = new ventasServices(urlServiceHana);
            var articuloListHANA = (tipoTablaSAP == TipoTablaSAP.OITM ? hanaService.articulo.Select(x => new AuthorizationItem() { Code = x.ItemCode, Name = x.ItemName })
                                                                     : hanaService.cliente.Select(x => new AuthorizationItem() { Code = x.CardCode, Name = x.CardName })).ToList();

            var articulosPermitidos = new PhartecContext().UsuarioPermisoItems.Where(x => x.PermisoVisualizacion == true && x.TipoTablaSAP == (int)tipoTablaSAP && x.UsuarioId == usuarioId && usuarioId != null).ToList().Select(x => x.Code).ToList();
            articuloListHANA.ForEach(x => x.HasAuthorization = articulosPermitidos.Contains(x.Code));
            return articuloListHANA;
        }

        public List<string> GetAuthorizedItemList(TipoTablaSAP tipoTablaSAP, int? usuarioId)
        {
            if (usuarioId == null || usuarioId == 0)
                return new List<string>();

            return new PhartecContext().UsuarioPermisoItems.Where(x => x.TipoTablaSAP == (int)tipoTablaSAP
                                                                                      && x.UsuarioId == usuarioId
                                                                                      && x.PermisoVisualizacion == true)
                                                                                      .Select(x => x.Code).ToList();
        }

        public void FilterOnlyAuthorizedItems(ref List<Venta> listaFinal, TipoTablaSAP tipoTablaSAP, int? usuarioId)
        {
            var authorizedItemList = GetAuthorizedItemList(tipoTablaSAP, usuarioId);
            listaFinal = listaFinal.Select(x => ((tipoTablaSAP == TipoTablaSAP.OITM ? authorizedItemList.Contains(x.ItemCode) : tipoTablaSAP == TipoTablaSAP.OCRD ? authorizedItemList.Contains(x.CardCode) : throw new Exception("Tipo tabla SAP no  controlada")) ? x : null)).Where(x => x != null).ToList();
        }
    }
}