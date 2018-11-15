using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    public class HistorialController : Controller
    {
        private Solicitud mSolicitud = new Solicitud();
        private AccionModel mAccion = new AccionModel();

        public ActionResult Index(string from = null, string to = null, string accion = null)
        {
            List<SolicitudListModel> resultado = mSolicitud.listarHistorialXUsuario(SessionHelper.GetUser());

            if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && string.IsNullOrEmpty(accion))
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && string.IsNullOrEmpty(accion))
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    && u.FechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(accion))
                  resultado = resultado.FindAll(u => u.accion.Equals(accion) &&
                      (u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                       && u.FechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture)));
            else if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(accion))
                resultado = resultado.FindAll(u => u.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture) && u.accion.Equals(accion));
            else if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(accion))
                resultado = resultado.FindAll(u => u.accion.Equals(accion));

            var listActions = mAccion.listar().Select(x => new SelectListItem
            {
                Text = x.descripcion,
                Value = x.accion
            });

            ViewBag.accion = listActions;
            return View(resultado);
        }

    }
}
