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
    [Autorization]
    public class MigracionController : Controller
    {
        private MigracionLog mMigracion = new MigracionLog();

        public ActionResult Index(int? DocId = null, string from = null, string to = null, string m = null)
        {
            var result = mMigracion.listar();

            if (DocId != null)
                result = result.FindAll(mi => mi.DocumentoId == (int)DocId);
            else if (DocId == null && !string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
                result = result.FindAll(mi => mi.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (DocId == null && !string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                result = result.FindAll(mi => mi.FechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                     && mi.FechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture));

            TempData["clicked"] = m;
            return View(result);
        }

    }
}
