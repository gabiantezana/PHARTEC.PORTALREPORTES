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
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private Menu menu = new Menu();
        private Notificacion mNotificacion = new Notificacion();
        private Usuario mUsuario = new Usuario();
        private Solicitud mSolicitud = new Solicitud();
        private Configuracion mConfiguracion = new Configuracion();
        private MenuRol mAccesos = new MenuRol();

        public ActionResult Index()
        {
            if (SessionHelper.GetUser() == 0)
            {
                return RedirectToAction("Index", "Autentificacion");
            }
            else if (SessionHelper.GetUser() > 0 && Session[Constantes.SESSION_USUARIO] == null)
            {
                var objUsuario = mUsuario.obtener(SessionHelper.GetUser());
                Session[Constantes.SESSION_USUARIO] = objUsuario;


                if (Session[Constantes.CONFIGURACION] == null)
                {
                    var objConfiguracion = mConfiguracion.obtener();
                    Session[Constantes.CONFIGURACION] = objConfiguracion;
                }

                if (Session[Constantes.ACCESOS] == null)
                {
                    var objAccesos = mAccesos.ObtenerXRol(objUsuario.Rol_id);
                    Session[Constantes.ACCESOS] = objAccesos;
                }
            }

            Session.Remove(Constantes.SESSION_FROM_PENDENTS);

            //ViewBag.TotalSPE = mSolicitud.listarXUsuario(SessionHelper.GetUser()).Count;
            //ViewBag.TotalSEP = mSolicitud.solXUsuRegistradas(SessionHelper.GetUser());
            //ViewBag.TotalSAP = mSolicitud.solXUsuAprobadas(SessionHelper.GetUser());
            //ViewBag.TotalSRE = mSolicitud.solXUsuRechazadas(SessionHelper.GetUser());

            //ViewBag.TotalSAP_PR = mSolicitud.solXUsuSAP(SessionHelper.GetUser(), EstadoSolicitud.RegistradaEnSAP);
            //ViewBag.TotalSAP_PO = mSolicitud.solXUsuSAP(SessionHelper.GetUser(), EstadoSolicitud.OrdenDeCompra);
            //ViewBag.TotalSAP_DN = mSolicitud.solXUsuSAP(SessionHelper.GetUser(), EstadoSolicitud.IngresoAlmacen);

            ViewBag.Notificaciones = mNotificacion.obtenerMessagesPendXUsuario(SessionHelper.GetUser());
            return View();
        }

        [ChildActionOnly]
        public ActionResult MenuPrincipal()
        {
            return PartialView(menu.listar());
        }

        [ChildActionOnly]
        public ActionResult Notificaciones()
        {
            return PartialView(mNotificacion.obtenerMessagesPendXUsuario(SessionHelper.GetUser()));
        }

        public ActionResult ViewReq(int id, int? nId = null, string type = null)
        {
            mNotificacion.updateEstadoNotificacion((int)nId);

            if (type != null && type.Equals("info"))
                Session[Constantes.SESSION_FROM_PENDENTS] = true;
            else
                Session.Remove(Constantes.SESSION_FROM_PENDENTS);
            return RedirectToAction("Details", "Solicitud", new { id = id });
        }

        public ActionResult UpdateNotificacion(int id)
        {
            mNotificacion.updateEstadoNotificacion(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
