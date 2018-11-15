using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Casuarinas.Helpers;
using Model;

namespace Casuarinas.Controllers
{
    [Autorization]
    public class SolicitudController : Controller
    {
        private Solicitud mSolicitud = new Solicitud();
        private TipoItem mTipoItem = new TipoItem();
        private Empresa mEmpresa = new Empresa();
        private Articulo mArticulo = new Articulo();
        private Usuario mUsuario = new Usuario();
        private CentroCosto mCentroCosto = new CentroCosto();
        private Notificacion mWebNotificacion = new Notificacion();
        private MenuRol mPermisos = new MenuRol();
        private SolDOriginal mSolOriginal = new SolDOriginal();

        // VIEW - Lista completa
        public ActionResult Index(string search = null, string from = null, string to = null)
        {
            List<Solicitud> resultado = mSolicitud.listar(SessionHelper.GetUser());

            if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && string.IsNullOrEmpty(search))
                resultado = resultado.FindAll(u => u.fechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && string.IsNullOrEmpty(search))
                resultado = resultado.FindAll(u => u.fechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    && u.fechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(search))
                resultado = resultado.FindAll(u => u.fechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    && u.fechaRegistro <= DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture) &&
                    (u.Empresa_descripcion.ToUpper().Contains(search.Trim().ToUpper()) || u.CentroCosto_descripcion.ToUpper().Contains(search.Trim().ToUpper())));
            else if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(search))
                resultado = resultado.FindAll(u => u.fechaRegistro >= DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture) &&
                    (u.Empresa_descripcion.ToUpper().Contains(search.Trim().ToUpper()) || u.CentroCosto_descripcion.ToUpper().Contains(search.Trim().ToUpper())));
            else if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(search))
                resultado = resultado.FindAll(u => u.Empresa_descripcion.ToUpper().Contains(search.Trim().ToUpper()) || u.CentroCosto_descripcion.ToUpper().Contains(search.Trim().ToUpper()));

            HttpContext.Session.Remove(Constantes.SOLICITUD_SESSION_DETAIL);
            HttpContext.Session.Remove(Constantes.SESSION_FROM_PENDENTS);

            return View(resultado);
        }

        // VIEW - Formulario Inserción y edición
        public ActionResult Form(int id = 0)
        {
            var objSolicitud = id > 0 ? mSolicitud.obtener(id) : mSolicitud;

            //Obtener la lista de tipos de item
            var listTipoItem = mTipoItem.listar().Select(x => new SelectListItem
            {
                Text = x.descripcion,
                Value = x.tipo,
                Selected = (x.tipo == objSolicitud.tipoSolicitud)
            });

            var details = objSolicitud.SolicitudDetalle.ToList();
            if (HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL] != null && details != null)
                details = fillListDataDetails(true, details);
            else if (HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL] == null && details != null)
                details = fillListDataDetails(false, details);

            //Obtener la sesión del usuario logueado si es CREACIÓN
            //if (id == 0)
            //{
            //    Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
            //    if (objCurrentUser == null)
            //    {
            //        SessionHelper.DestroyUserSession();
            //        return Redirect("~/home/index");
            //    }
            //    objSolicitud.Usuario_id = objCurrentUser.id;
            //    objSolicitud.Usuario_nombre = objCurrentUser.nombre;
            //    objSolicitud.Usuario_correo = objCurrentUser.correo;

            //    var empList = objCurrentUser.Empresas;

            //    if (empList != null && empList.Count == 1)
            //    {
            //        ViewBag.empresa = empList.Select(x => new SelectListItem
            //        {
            //            Text = x.descripcion,
            //            Value = x.id.ToString(),
            //            Selected = true
            //        });

            //        var empId = empList.Take(1).Single().id;

            //        //var costList = (from c in objCurrentUser.CentrosCosto
            //        //                    where c.Empresa_id == empId
            //        //                    select c).ToList();

            //        if(costList != null && costList.Count == 1){
            //            ViewBag.centroCosto = costList.Select(x => new SelectListItem
            //            {
            //                Text = x.descripcion,
            //                Value = x.id.ToString(),
            //                Selected = true
            //            });
            //        }
            //        else if (costList != null && costList.Count > 1)
            //        {
            //            ViewBag.centroCosto = costList.Select(x => new SelectListItem
            //            {
            //                Text = x.descripcion,
            //                Value = x.id.ToString(),
            //            });
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.empresa = empList.Select(x => new SelectListItem
            //        {
            //            Text = x.descripcion,
            //            Value = x.id.ToString(),
            //            Selected = (x.id == objSolicitud.empresa)
            //        });
            //    }
            //}
            //else
            //{
            //    //Si es edición pre cargar los datos
            //    ViewBag.empresa = mEmpresa.listar().Select(x => new SelectListItem
            //    {
            //        Text = x.descripcion,
            //        Value = x.id.ToString(),
            //        Selected = (x.id == objSolicitud.empresa)
            //    });

            //    ViewBag.centroCosto = mCentroCosto.listarXEmpresa(objSolicitud.empresa).Select(x => new SelectListItem
            //    {
            //        Text = x.descripcion,
            //        Value = x.id.ToString(),
            //        Selected = (x.id == objSolicitud.centroCosto)
            //    });
            //}

            ViewBag.Detalles = details;
            ViewBag.tipoSolicitud = listTipoItem;
            return View(objSolicitud != null ? objSolicitud : mSolicitud);
        }

        // VIEW - Detalles
        public ActionResult Details(int id)
        {
            var objSolicitud = mSolicitud.obtener(id);

            foreach (var item in objSolicitud.SolicitudEstado)
            {
                item.Usuario_nombre = mUsuario.getName(item.Usuario);
            }

            if (objSolicitud != null && objSolicitud.CreadoPor != objSolicitud.ActualizadoPor)
                ViewBag.Historico = mSolOriginal.listarXIdSol(id);

            return View(objSolicitud);
        }

        // PARTIAL VIEW - Lista Detalle de solicitud
        public PartialViewResult DataDetalle(string temporal_id = null, string codigo = null, string descripcion = null, string fecha = null,
            int cantidad = 0, string comentario = null, bool? deleteAll = null, string deleteId = null)
        {
            List<SolicitudDetalle> objList = (List<SolicitudDetalle>)HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL];

            if (descripcion != null && fecha != null && string.IsNullOrEmpty(temporal_id))
            {
                if (objList == null)
                    objList = new List<SolicitudDetalle>();

                SolicitudDetalle objDetail = new SolicitudDetalle();
                objDetail.temporal_id = Guid.NewGuid().ToString("N");
                objDetail.Articulo_codigosap = codigo;
                objDetail.descripcion = descripcion;
                objDetail.fechaNecesaria = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                objDetail.cantidad = cantidad;
                objDetail.comentario = comentario;

                objList.Add(objDetail);
            }
            else if (descripcion != null && fecha != null && !string.IsNullOrEmpty(temporal_id))
            {
                foreach (var item in objList.Where(d => d.temporal_id == temporal_id))
                {
                    item.Articulo_codigosap = codigo;
                    item.descripcion = descripcion;
                    item.fechaNecesaria = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    item.cantidad = cantidad;
                    item.comentario = comentario;
                }
            }
            else if (deleteId != null)
            {
                for (int i = objList.Count - 1; i >= 0; i--)
                {
                    var item = objList[i];
                    if (item.temporal_id.Equals(deleteId))
                    {
                        objList.RemoveAt(i);
                        break;
                    }
                }
            }
            else if (deleteAll != null)
            {
                if (objList == null)
                    objList = new List<SolicitudDetalle>();
                
                objList.Clear();
            }

            HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL] = objList;
            ViewBag.Detalles = objList;
            return PartialView();
        }


        // METHOD - SAVE (WHEN EDIT AND REGISTER)
        [HttpPost]
        public JsonResult Guardar(Solicitud model)
        {
            //Instanciar respuesta
            var respuesta = new ResponseModel
            {
                respuesta = true,
                redirect = "solicitud/form/" + model.id,
                error = "",
                error2 = ""
            };

            if (model.id == 0)
            {
                //Validar seleccion de empresa
                if (model.empresa <= 0)
                {
                    ModelState.AddModelError(SaveMessageValidation.EMPRESA_MODEL_ERROR, SaveMessageValidation.SELECCIONE_EMPRESA);
                    respuesta.respuesta = false;
                    respuesta.error = SaveMessageValidation.SELECCIONE_EMPRESA;

                    return Json(respuesta);

                }//Validar que la empresa se encuentre activa
                else if(!mEmpresa.isActive(model.empresa))
                {
                    ModelState.AddModelError(SaveMessageValidation.EMPRESA_MODEL_ERROR, SaveMessageValidation.SELECCIONE_EMPRESA);
                    respuesta.respuesta = false;
                    respuesta.error = "La empresa seleccionada se encuentra inactiva, contacte al administrador.";

                    return Json(respuesta);
                }

                //Validar seleccion de centro de costo
                if (model.centroCosto <= 0)
                {
                    ModelState.AddModelError(SaveMessageValidation.CENTROCOSTO_MODEL_ERROR, SaveMessageValidation.SELECCIONE_CENTROCOSTO);
                    respuesta.respuesta = false;
                    respuesta.error = SaveMessageValidation.SELECCIONE_CENTROCOSTO;

                    return Json(respuesta);

                } //Validar que el centro de costo se encuentre activo
                else if(!mCentroCosto.isActive(model.centroCosto))
                {
                    ModelState.AddModelError(SaveMessageValidation.CENTROCOSTO_MODEL_ERROR, SaveMessageValidation.SELECCIONE_CENTROCOSTO);
                    respuesta.respuesta = false;
                    respuesta.error = "El centro de costo seleccionado se encuentra inactivo, contacte al administrador.";

                    return Json(respuesta);
                }
            }
            else
            {
                model.empresa = mSolicitud.obtener(model.id).empresa;
                model.centroCosto = mSolicitud.obtener(model.id).centroCosto;
            }

            //Obtener los detalles desde la sesión
            var details = (List<SolicitudDetalle>)HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL];

            //Asignar los detalles al objeto solicitud
            if (details != null && details.Count > 0)
            {
                foreach (var item in details)
                {
                    item.Articulo_id = mArticulo.obtenerId(item.Articulo_codigosap);
                    item.Articulo = new Articulo { id = (int) item.Articulo_id };
                    model.SolicitudDetalle.Add(item);
                }

                model.Usuario = new Usuario { id = model.Usuario_id };
                model.estado = EstadoSolicitud.INICIAL_REGISTRADA;

                //asignar los niveles de aprobación si la solicitud va a crearse
                if (model.id == 0)
                {
                    var niveles = mCentroCosto.obtener(model.centroCosto).CentroCostoNivel.OrderBy(n => n.prioridad).ToList();
                    if (niveles != null && niveles.Count > 0)
                    {
                        foreach (var item in niveles)
                        {
                            SolicitudEstado objEstado = new SolicitudEstado();
                            objEstado.Usuario = item.Usuario_id;
                            objEstado.accion = EstadoSolicitud.INICIAL_REGISTRADA;
                            objEstado.fechaRegistro = model.fechaRegistro;
                            objEstado.activo = item.prioridad == 1 ? true : false;
                            objEstado.prioridad = item.prioridad;
                            model.SolicitudEstado.Add(objEstado);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(SaveMessageValidation.NAPROBACION_MODEL_ERROR, SaveMessageValidation.SELECCIONE_NAPROBACION);
                        respuesta.respuesta = false;
                        respuesta.error = SaveMessageValidation.SELECCIONE_NAPROBACION;
                    }
                }
            }
            else
            {
                ModelState.AddModelError(SaveMessageValidation.DETALLES_MODEL_ERROR, SaveMessageValidation.SELECCIONE_DETALLES);
                respuesta.respuesta = false;
                respuesta.error = SaveMessageValidation.SELECCIONE_DETALLES;
            }

            //si todos los datos son correctos registrar el objeto
            if (ModelState.IsValid || model.id > 0)
            {
                model.usuario_curr = SessionHelper.GetUser();
                bool newObject = model.id == 0;

                var result = model.Guardar();
                if (String.IsNullOrEmpty(result))
                {
                    TempData[Constantes.TEMPDATA_MESSAGE] = Constantes.SUCCESS_MESSAGE;
                    respuesta.redirect = newObject ? "solicitud/index" : "solicitud/Details/" + model.id;

                    if (newObject)
                    {
                        Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
                        Configuracion objConfiguracion = (Configuracion)Session[Constantes.CONFIGURACION];

                        //Envio EMAIL
                        var userId = (from e in model.SolicitudEstado
                                      where e.activo == true
                                      select e).Single().Usuario;
                        var sendUser = mUsuario.obtener(userId);

                        //Si el usuario aprobador es el mismo que está registrando aprobar automáticamente la solicitud
                        if (userId == SessionHelper.GetUser())
                        {
                            model.id = mSolicitud.obtenerUltimoId(userId);
                            mSolicitud.Aprobar(userId);
                        }

                        //Actualizar el numero de notificaciones pendientes del usuario
                        mWebNotificacion.updateNumPendMessages(sendUser.id, model, "info");
                        respuesta.extraValue1 = sendUser.id.ToString();

                        if (sendUser != null && !string.IsNullOrEmpty(sendUser.correo))
                        {
                            EmailModel email = new EmailModel();
                            email.ToEmail = sendUser.correo;
                            email.Subject = "MSS SEIDOR WEB REQ - NUEVA SOLICITUD N° " + model.id;
                            email.userName = objCurrentUser.nombre;
                            email.solID = model.id;
                            var sendEmail = new EmailHelper().sendEmail(objConfiguracion, email, Constantes.EMAIL_NUEVO_REGISTRO);

                            if (string.IsNullOrEmpty(sendEmail))
                                TempData[Constantes.TEMPDATA_MESSAGE_EMAIL] = Constantes.SUCCESS_MESSAGE;
                            else
                                TempData[Constantes.TEMPDATA_MESSAGE_EMAIL_ERROR] = "Error notificando al usuario " + sendUser.nombre + " (aprobador): " + sendEmail;
                        }
                        else if (sendUser != null && !string.IsNullOrEmpty(sendUser.nombre))
                        {
                            TempData[Constantes.TEMPDATA_MESSAGE_EMAIL_ERROR] = "El Usuario " + sendUser.nombre + " (aprobador) no tiene un correo registrado. No se le pudo notificar.";
                        }
                    }
                    else
                    {
                        //Actualizar el numero de notificaciones pendientes del usuario
                        mWebNotificacion.updateNumPendMessages(SessionHelper.GetUser(), model, "warning");
                    }
                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.error = "DBError. " + result;
                }
            }

            return Json(respuesta);
        }

        //Utils - Llenar la lista de detalles en sesion
        private List<SolicitudDetalle> fillListDataDetails(bool sessionExists, List<SolicitudDetalle> existentList)
        {
            List<SolicitudDetalle> currentList;

            if (sessionExists)
                currentList = (List<SolicitudDetalle>)HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL];
            else
                currentList = new List<SolicitudDetalle>();

            foreach (var item in existentList)
            {
                try
                {
                    var founded = currentList.Find(n => n.temporal_id.Equals(item.temporal_id));

                    if (founded == null)
                    {
                        var objDetail = new SolicitudDetalle();
                        objDetail.temporal_id = item.temporal_id;
                        objDetail.Articulo_codigosap = item.Articulo_codigosap;
                        objDetail.descripcion = item.descripcion;
                        objDetail.fechaNecesaria = item.fechaNecesaria;
                        objDetail.cantidad = item.cantidad;
                        objDetail.comentario = item.comentario;
                        currentList.Add(objDetail);
                    }
                }
                catch (Exception)
                {
                }
            }

            HttpContext.Session[Constantes.SOLICITUD_SESSION_DETAIL] = currentList;
            return currentList;
        }

    }
}
