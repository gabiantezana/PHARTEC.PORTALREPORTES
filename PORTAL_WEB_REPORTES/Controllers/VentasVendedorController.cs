using Casuarinas.Helpers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amba.SpreadsheetLight;
using MSS_REPORTES_TEST.services.ventas.ventasServices;

namespace Casuarinas.Controllers
{
	[Autorization]
	public class VentasVendedorController : Controller
	{
		private Moneda mMoneda = new Moneda();
		public ActionResult Index(string sucursal = null, DateTime? desde = null, DateTime? hasta = null,
			string moneda = null, string divisionx = null, string subdivisionx = null, string lineax = null)
		{
			//Listar los tipo de moneda
			var listMoneda = mMoneda.listar().Select(x => new SelectListItem
			{
				Text = x.descripcion,
				Value = x.valor,
				Selected = x.valor.Equals("USD")
			});
			ViewBag.moneda = listMoneda;

			//Obtener al usuario actual
			Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
			if (objCurrentUser == null)
			{
				SessionHelper.DestroyUserSession();
				return Redirect("~/home/index");
			}

			//Obtener las sucursales asignadas del usuario actual
			var sucList = objCurrentUser.Sucursales;
			var listSucursales = sucList.Select(x => new SelectListItem
			{
				Text = x.descripcion,
				Value = x.descripcion,
				Selected = true
			});
			ViewBag.sucursal = listSucursales;

			//Obtener las divisiones asignadas del usuario actual
			var divList = objCurrentUser.Divisiones;
			ViewBag.divisionx = divList.Select(x => new SelectListItem
			{
				Text = x.descripcion,
				Value = x.codigoSap,
				Selected = x.descripcion.Equals("Todas")
			});

			//Obtener las subdivisiones asignadas del usuario actual
			var subList = objCurrentUser.Subdivisiones;
			ViewBag.subdivisionx = subList.Select(x => new SelectListItem
			{
				Text = x.descripcion,
				Value = x.codigoSap,
				Selected = x.descripcion.Equals("Todas")
			});

			//Obtener las líneas asignadas del usuario actual
			var linList = objCurrentUser.Lineas;
			ViewBag.lineax = linList.Select(x => new SelectListItem
			{
				Text = x.descripcion,
				Value = x.codigoSap,
				Selected = x.descripcion.Equals("Todas")
			});

            //Servicio OData
            string _url = System.Configuration.ConfigurationManager.AppSettings.Get("URLoDataService");
            Uri myUri = new Uri(_url);            
			ventasServices myService = new ventasServices(myUri);

			//Verificar si el usuario actual es vendedor
			if (objCurrentUser.Rol.id == 2)//rol 2=vendedor
			{
				ViewBag.esVendedor = true;
			}
			else
			{
				ViewBag.esVendedor = false;
			}

			//Verificar si todos los filtros estan vacíos, mostrar la vista
			if (sucursal == null && divisionx == null && subdivisionx == null && lineax == null)
			{
				return View();
			}
			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasType> resultadoDolares = null;
			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasSolesType> resultadoSoles = null;
			if (moneda == "USD")
			{
				resultadoDolares = from i in myService.ventas
								   where (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap)))
								   &&(i.Fecha >= desde && i.Fecha <= hasta) &&
								   (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
								   (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
								   (lineax == "Todas" || i.OcrCode3ST.Equals(lineax))
								   select i;
			}
			else
			{
				resultadoSoles = from i in myService.ventasSoles
								 where (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap)))
								 &&(i.Fecha >= desde && i.Fecha <= hasta) &&
								 (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
								 (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
								 (lineax == "Todas" || i.OcrCode3ST.Equals(lineax))
								 select i;
			}

			List<Venta> listaTmpSinAgrupar = ObtenerListaSinAgrupar((resultadoDolares != null) ? resultadoDolares.ToList() : null,
			(resultadoSoles != null) ? resultadoSoles.ToList() : null,
			sucList, divList, subList, linList, sucursal);



			//Agrupar los valores
			var resultadoAgrupado = from r in listaTmpSinAgrupar
									group r by new
									{
										SlpCode = r.SlpCode,
										SlpName = r.SlpName,
										Sucursal = r.Sucursal,
										//OcrCode2 = r.OcrCode2,
										//OcrCode = r.OcrCode
									} into g
									orderby g.Key.SlpName
									select new
									{
										SlpCode = g.Key.SlpCode,
										SlpName = g.Key.SlpName,
										Sucursal = g.Key.Sucursal,
										//OcrCode2 = g.Key.OcrCode2,
										//OcrCode = g.Key.OcrCode,
										Quantity = g.Sum(x => x.Quantity),
										TotalCosto = g.Sum(x => x.TotalCosto),
										TotalVenta = g.Sum(x => x.TotalVenta),
										Margen = g.Sum(x => x.Margen)
									};
			List<Venta> listaFinal = new List<Venta>();

			//Crear lista de tipo Venta, ya con los valores agrupados
			foreach (var itemVenta in resultadoAgrupado.ToList())
			{
				var v = new Venta();
				v.SlpCode = itemVenta.SlpCode;
				v.SlpName = itemVenta.SlpName;
				v.Sucursal = itemVenta.Sucursal;
				//v.OcrCode = itemVenta.OcrCode;
				//v.OcrCode2 = itemVenta.OcrCode2;
				v.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
				v.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
				v.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());
				if (v.TotalVenta != 0)
				{
					v.Margen = ((v.TotalVenta - v.TotalCosto) / v.TotalVenta) * 100;
				}
				else
				{
					v.Margen = -100;
				}
				v.Margen = Math.Round(decimal.Parse(v.Margen.ToString()), 2);

				v.MargenEstandarizado = ((v.TotalVenta * v.Margen) / 0.5M) / 100;
				v.MargenEstandarizado = Math.Round(decimal.Parse(v.MargenEstandarizado.ToString()), 2);

				listaFinal.Add(v);
			}
			Totales totales = CalcularTotales(listaFinal);

			listaFinal = listaFinal.OrderByDescending(o => o.TotalVenta).ToList();

			ViewBag.resultado = listaFinal;
			ViewBag.totales = totales;

			Session["listaVentas"] = listaFinal;


			string strDesde = "";
			string strHasta = "";
			if (desde != null)
			{
				strDesde = DateTime.Parse(desde.ToString()).ToString("yyyy-MM-dd");
			}
			if (hasta != null)
			{
				strHasta = DateTime.Parse(hasta.ToString()).ToString("yyyy-MM-dd");
			}

			//filtros
			Filtro filtros = new Filtro();
			filtros.Sucursal = sucursal;
			filtros.Desde = strDesde;
			filtros.Hasta = strHasta;
			filtros.Moneda = moneda;
			filtros.Divisionx = divisionx;
			filtros.Subdivisionx = subdivisionx;
			filtros.Lineax = lineax;
			filtros.SellerCode = null;
			filtros.Tab = "1";

			ViewBag.filtros = filtros;

			return View();

		}

		Totales CalcularTotales(List<Venta> listaFinal)
		{
			Totales totales = new Totales();
			foreach (var itemVenta in listaFinal)
			{
				totales.TotalVentas += double.Parse(itemVenta.TotalVenta.ToString());
				totales.TotalCompras += double.Parse(itemVenta.TotalCosto.ToString());
			}
			totales.TotalMargen = ((totales.TotalVentas - totales.TotalCompras) / totales.TotalVentas) * 100;
			totales.TotalMargenEstandarizado = ((totales.TotalVentas * totales.TotalMargen) / 0.50) / 100;
			return totales;
		}

		public ActionResult Form(string sucursalLinea = null, string subdivisionxLinea = null,
			string lineaxLinea = null, string divisionxLinea = null, 
			string itemCode = null, string slpCode = null, 
			string cardCode = null, string sucursal = null, DateTime desde = new DateTime(), DateTime hasta = new DateTime(),
			string divisionx = null, string subdivisionx = null, string lineax = null, string moneda = null)
		{

			Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];

			var sucList = objCurrentUser.Sucursales;

			var divList = objCurrentUser.Divisiones;

			var subList = objCurrentUser.Subdivisiones;

			var linList = objCurrentUser.Lineas;

            string _url = System.Configuration.ConfigurationManager.AppSettings.Get("URLoDataService");
            Uri myUri = new Uri(_url);
			ventasServices myService = new ventasServices(myUri);

			//enviar dato de tipo de usuario vendedor, 
			if (objCurrentUser.Rol.id == 2) //rol 2 = vendedor
			{
				ViewBag.esVendedor = true;
			}
			else
			{
				ViewBag.esVendedor = false;
			}

			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasType> resultadoDolares = null;
			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasSolesType> resultadoSoles = null;
			if (moneda == "USD")
			{
				resultadoDolares = from i in myService.ventas
								   where
									 (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
                                   //MODIFICADO LA LINEA DE ARRIBA, CAMBIE SUCRUSALLINEA POR SUCURSAL
                                   &&(i.Fecha >= desde && i.Fecha <= hasta) &&
								   (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
                                   //MODIFICADO LA LINEA DE ARRIBA, CAMBIE DIVISIONXLINEA POR DIVISIONX
                                   (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
								   (lineax == "Todas" || i.OcrCode3ST.Equals(lineax))
								   && (slpCode == null || i.SlpCode == int.Parse(slpCode))
								   select i;

			}
			else
			{
				resultadoSoles = from i in myService.ventasSoles
								 where
								   (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
                                 //MODIFICADO LA LINEA DE ARRIBA, CAMBIE SUCRUSALLINEA POR SUCURSAL
								 &&(i.Fecha >= desde && i.Fecha <= hasta) &&
								  (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
                                  //MODIFICADO LA LINEA DE ARRIBA, CAMBIE DIVISIONXLINEA POR DIVISIONX
                                  (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
								   (lineax == "Todas" || i.OcrCode3ST.Equals(lineax))
								 && (slpCode == null || i.SlpCode == int.Parse(slpCode))
								 select i;

			}

            List<Venta> listaTmpSinAgrupar = ObtenerListaSinAgrupar((resultadoDolares != null) ? resultadoDolares.ToList() : null,
			(resultadoSoles != null) ? resultadoSoles.ToList() : null,
			sucList, divList, subList, linList, sucursal);
            
			//agrupar los valores, por producto
			var resultadoAgrupado = from r in listaTmpSinAgrupar
									group r by new
									{
										ItemCode = r.ItemCode,
										Description = r.Description,
										OcrCode3 = r.OcrCode3,
										//Documento = r.Documento
									} into g
									select new
									{
										ItemCode = g.Key.ItemCode,
										Description = g.Key.Description,
										OcrCode3 = g.Key.OcrCode3,
										//Documento = g.Key.Documento,
									
										Quantity = g.Sum(x => x.Quantity),
										TotalCosto = g.Sum(x => x.TotalCosto),
										TotalVenta = g.Sum(x => x.TotalVenta),
										Margen = g.Sum(x => x.Margen)
									};

			List<Venta> listaFinalAgrupadaPorProducto = new List<Venta>();

			//crear lista de tipo Venta, ya con los valores agrupados por prodcto
			foreach (var itemVenta in resultadoAgrupado.ToList())
			{
				var v = new Venta();
				v.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
				v.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
				v.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());
				if (v.TotalVenta != 0)
				{
					v.Margen = ((v.TotalVenta - v.TotalCosto) / v.TotalVenta) * 100;

				}
				else
				{
					v.Margen = -100;

				}
				v.Margen = Math.Round(decimal.Parse(v.Margen.ToString()), 2);

				v.ItemCode = itemVenta.ItemCode;
				v.Description = itemVenta.Description;
				v.OcrCode3 = itemVenta.OcrCode3;
				//v.Documento = itemVenta.Documento;
			
				try
				{
					v.VentaUnit = itemVenta.TotalVenta / itemVenta.Quantity;
					v.CostoUnit = itemVenta.TotalCosto / itemVenta.Quantity;
					v.Pvu = itemVenta.TotalVenta / itemVenta.Quantity;
				}
				catch (Exception e)
				{
					v.VentaUnit = 0;
					v.CostoUnit = 0;
					v.Pvu = 0;

				}

				v.MargenEstandarizado = ((v.TotalVenta * v.Margen) / 0.5M)/100;
				v.MargenEstandarizado = Math.Round(decimal.Parse(v.MargenEstandarizado.ToString()), 2);


				listaFinalAgrupadaPorProducto.Add(v);
			}

			Totales totalesPorProducto = CalcularTotales(listaFinalAgrupadaPorProducto);

			listaFinalAgrupadaPorProducto = listaFinalAgrupadaPorProducto.OrderByDescending(o => o.TotalVenta).ToList();


			ViewBag.resultado = listaFinalAgrupadaPorProducto;
			ViewBag.totales = totalesPorProducto;

			//esta variable de sesion se usará para el botón Exportar
			Session["listaVentasPorProducto"] = listaFinalAgrupadaPorProducto;

			//---------agrupar los valores, por cliente
			var resultadoAgrupadoPorCliente = from r in listaTmpSinAgrupar
											  group r by new
											  {
												  SlpName = r.SlpName,
												  CardCode = r.CardCode,
												  CardName = r.CardName,
											  } into g
											  orderby g.Key.SlpName
											  select new
											  {
												  SlpName = g.Key.SlpName,
												  CardCode = g.Key.CardCode,
												  CardName = g.Key.CardName,

												  Quantity = g.Sum(x => x.Quantity),
												  TotalCosto = g.Sum(x => x.TotalCosto),
												  TotalVenta = g.Sum(x => x.TotalVenta),
												  Margen = g.Sum(x => x.Margen)
											  };

			List<Venta> listaFinalAgrupadaPorCliente = new List<Venta>();

			//crear lista de tipo Venta, ya con los valores agrupados por cliente
			foreach (var itemVenta in resultadoAgrupadoPorCliente.ToList())
			{
				var v = new Venta();
				v.SlpName = itemVenta.SlpName;
				v.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
				v.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
				v.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());
				if (v.TotalVenta != 0)
				{
					v.Margen = ((v.TotalVenta - v.TotalCosto) / v.TotalVenta) * 100;

				}
				else
				{
					v.Margen = -100;

				}
				v.Margen = Math.Round(decimal.Parse(v.Margen.ToString()), 2);

				v.CardCode = itemVenta.CardCode;
				v.CardName = itemVenta.CardName;

				try
				{
					v.Pvu = itemVenta.TotalVenta / itemVenta.Quantity;

				}
				catch (Exception e)
				{
					v.Pvu = 0;
				}

				v.MargenEstandarizado = ((v.TotalVenta * v.Margen) / 0.5M) / 100;
				v.MargenEstandarizado = Math.Round(decimal.Parse(v.MargenEstandarizado.ToString()), 2);

				listaFinalAgrupadaPorCliente.Add(v);
			}

			Totales totalesPorCliente = CalcularTotales(listaFinalAgrupadaPorCliente);

            listaFinalAgrupadaPorCliente = listaFinalAgrupadaPorCliente.OrderByDescending(o => o.TotalVenta).ToList();


			ViewBag.resultadoPorCliente = listaFinalAgrupadaPorCliente;
			ViewBag.totalesPorCliente = totalesPorCliente;

			//esta variable de sesion se usará para el botón Exportar
			Session["listaVentasPorCliente"] = listaFinalAgrupadaPorCliente;


			//enviar filtros aplicados
			Filtro filtros = new Filtro();
			filtros.Sucursal = sucursal;
			filtros.Desde = desde.ToString("yyyy-MM-dd");
			filtros.Hasta = hasta.ToString("yyyy-MM-dd");
			filtros.Moneda = moneda;
			filtros.Divisionx = divisionx;
			filtros.Subdivisionx = subdivisionx;
			filtros.Lineax = lineax;
			filtros.CardCode = cardCode;
			filtros.ItemCode = null;
			filtros.SellerCode = null;
			filtros.Tab = "1";
            filtros.SlpCode = slpCode;

			ViewBag.filtros = filtros;

			return View();



		}


		public ActionResult Details(string itemCode = null, string slpCode = null, string documento = null,
			string cardCode = null, string sucursal = null, DateTime desde = new DateTime(), DateTime hasta = new DateTime(),
			string divisionx = null, string subdivisionx = null, string lineax = null, string moneda = null)
		{

			Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];

			var sucList = objCurrentUser.Sucursales;

			var divList = objCurrentUser.Divisiones;

			var subList = objCurrentUser.Subdivisiones;

			var linList = objCurrentUser.Lineas;


            string _url = System.Configuration.ConfigurationManager.AppSettings.Get("URLoDataService");
            Uri myUri = new Uri(_url);
			ventasServices myService = new ventasServices(myUri);

			List<string> sucursalesPorBuscar = new List<string>();


			//enviar dato de tipo de usuario vendedor, 
			if (objCurrentUser.Rol.id == 2) //rol 2 = vendedor
			{
				ViewBag.esVendedor = true;
			}
			else
			{
				ViewBag.esVendedor = false;
			}
			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasType> resultadoDolares = null;
			System.Linq.IQueryable<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasSolesType> resultadoSoles = null;

			if (documento == null) documento = "";

			if (moneda == "USD")//por dolares
			{
				if (cardCode == null || cardCode == "") //por producto
				{
                    resultadoDolares = from i in myService.ventas
                                        where
                                            (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
                                        &&(i.Fecha >= desde && i.Fecha <= hasta) &&
                                        (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
                                        (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
                                        (lineax == "Todas" || i.OcrCode3ST.Equals(lineax)) &&
                                        (documento == "" || i.Documento.Equals(documento)) &&
                                        (slpCode == null || i.SlpCode == int.Parse(slpCode)) &&  //AÑADIDO
                                        (itemCode == "" || i.ItemCode.Equals(itemCode))//<--aquí la condicion del producto

                                        select i;
				}
				else //por cliente
				{
					resultadoDolares = from i in myService.ventas
									   where
										 (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
									   &&(i.Fecha >= desde && i.Fecha <= hasta) &&
									   (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
									   (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
									   (lineax == "Todas" || i.OcrCode3ST.Equals(lineax)) &&
									   (documento == "" || i.Documento.Equals(documento)) &&
                                       (slpCode == null || i.SlpCode == int.Parse(slpCode)) &&  //AÑADIDO
                                       (cardCode == "" || i.CardCode.Equals(cardCode))//<--aquí la condición del cliente

									   select i;

				}
			}
			else//por soles
			{
				if (cardCode == null || cardCode == "") //por producto
				{
					resultadoSoles = from i in myService.ventasSoles
									 where
									   (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
									 &&(i.Fecha >= desde && i.Fecha <= hasta) &&
									 (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
									 (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
									 (lineax == "Todas" || i.OcrCode3ST.Equals(lineax)) &&
									 (documento == "" || i.Documento.Equals(documento)) &&
                                     (slpCode == null || i.SlpCode == int.Parse(slpCode)) &&  //AÑADIDO
                                     (itemCode == "" || i.ItemCode.Equals(itemCode))//<--aquí la condición del producto

									 select i;

				}
				else //por cliente
				{
					resultadoSoles = from i in myService.ventasSoles
									 where
									   (objCurrentUser.Rol.id != 2 || (i.SlpCode.Equals(objCurrentUser.codigo_sap))) //si es vendedor validar solo sus ventas, si no lo es esta condicion se ignora
									 &&(i.Fecha >= desde && i.Fecha <= hasta) &&
									 (divisionx == "Todas" || i.OcrCodeST.Equals(divisionx)) &&
									 (subdivisionx == "Todas" || i.OcrCode2ST.Equals(subdivisionx)) &&
									 (lineax == "Todas" || i.OcrCode3ST.Equals(lineax)) &&
									 (documento == ""  || i.Documento.Equals(documento)) &&
                                     (slpCode == null || i.SlpCode == int.Parse(slpCode)) &&  //AÑADIDO
                                    (cardCode == "" || i.CardCode.Equals(cardCode))//<--aquí la condición del cliente

									 select i;

				}
			}




			List<Venta> listaTmpSinAgrupar = ObtenerListaSinAgrupar((resultadoDolares != null) ? resultadoDolares.ToList() : null,
				(resultadoSoles != null) ? resultadoSoles.ToList() : null,
				sucList, divList, subList, linList, sucursal);




			//--DETALLE POR PRODUCTO
			if ((itemCode != null && itemCode != ""))
			{
				//----------agrupar los valores, por detalle
				var resultadoAgrupadoPorDetalle = from r in listaTmpSinAgrupar
												  group r by new
												  {
													  CardName = r.CardName,
													  ItemCode = r.ItemCode,
													  Description = r.Description,
													  OcrCode3 = r.OcrCode3,
													  SlpName = r.SlpName,
													  Fecha = r.Fecha,
													  Documento = r.Documento
												  } into g
												  orderby g.Key.CardName
												  select new
												  {
													  CardName = g.Key.CardName,
													  ItemCode = g.Key.ItemCode,
													  Description = g.Key.Description,
													  OcrCode3 = g.Key.OcrCode3,
													  SlpName = g.Key.SlpName,
													  Fecha = g.Key.Fecha,
													  Documento = g.Key.Documento,
													  Quantity = g.Sum(x => x.Quantity),
													  TotalCosto = g.Sum(x => x.TotalCosto),
													  TotalVenta = g.Sum(x => x.TotalVenta),
													  Margen = g.Sum(x => x.Margen)
												  };

				List<Venta> listaFinalAgrupadaPorDetalle = new List<Venta>();

				//crear lista de tipo Venta, ya con los valores agrupados por detalle
				foreach (var itemVenta in resultadoAgrupadoPorDetalle.ToList())
				{
					var v = new Venta();
					v.CardName = itemVenta.CardName;
					v.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
					v.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
					v.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());
					if (v.TotalVenta != 0)
					{
						v.Margen = ((v.TotalVenta - v.TotalCosto) / v.TotalVenta) * 100;

					}
					else
					{
						v.Margen = -100;

					}
					v.Margen = Math.Round(decimal.Parse(v.Margen.ToString()), 2);

					v.MargenEstandarizado = ((v.TotalVenta * v.Margen) / 0.5M) / 100;
					v.MargenEstandarizado = Math.Round(decimal.Parse(v.MargenEstandarizado.ToString()), 2);

					v.ItemCode = itemVenta.ItemCode;
					v.Description = itemVenta.CardName;//asignarle nombre de cliente, porque viene de pestaña producto
					v.OcrCode3 = itemVenta.OcrCode3;
					try
					{
						v.VentaUnit = itemVenta.TotalVenta / itemVenta.Quantity;
						v.CostoUnit = itemVenta.TotalCosto / itemVenta.Quantity;
						v.Pvu = itemVenta.TotalVenta / itemVenta.Quantity;
					}
					catch (Exception e)
					{
						v.VentaUnit = 0;
						v.CostoUnit = 0;
						v.Pvu = 0;

					}
					v.SlpName = itemVenta.SlpName;
					v.Fecha = itemVenta.Fecha;
					v.Documento = itemVenta.Documento;



					listaFinalAgrupadaPorDetalle.Add(v);
				}

				Totales totalesPorDetalle = CalcularTotales(listaFinalAgrupadaPorDetalle);

				listaFinalAgrupadaPorDetalle = listaFinalAgrupadaPorDetalle.OrderByDescending(o => o.TotalVenta).ToList();


				ViewBag.resultadoPorDetalle = listaFinalAgrupadaPorDetalle;
				ViewBag.totalesPorDetalle = totalesPorDetalle;

				//esta variable de sesion se usará para el botón Exportar
				Session["listaVentasPorDetalle"] = listaFinalAgrupadaPorDetalle;

				//enviar filtros aplicados
				Filtro filtros = new Filtro();
				filtros.Sucursal = sucursal;
				filtros.Desde = desde.ToString("yyyy-MM-dd");
				filtros.Hasta = hasta.ToString("yyyy-MM-dd");
				filtros.Moneda = moneda;
				filtros.Divisionx = divisionx;
				filtros.Subdivisionx = subdivisionx;
				filtros.Lineax = lineax;
				filtros.CardCode = cardCode;
				filtros.ItemCode = null;
				filtros.SellerCode = null;
				filtros.Tab = "3";

				ViewBag.filtros = filtros;

				return PartialView();

			}




			//-----DETALLE POR CLIENTE
			if (cardCode != null)
			{
				//----------agrupar los valores, por detalle
				var resultadoAgrupadoPorDetalle = from r in listaTmpSinAgrupar
												  group r by new
												  {
													  SlpName = r.SlpName,
													  ItemCode = r.ItemCode,
													  Description = r.Description,
													  OcrCode3 = r.OcrCode3,
													  CardName = r.CardName,
													  Fecha = r.Fecha,
													  Documento = r.Documento
												  } into g
												  orderby g.Key.SlpName
												  select new
												  {
													  SlpName = g.Key.SlpName,

													  ItemCode = g.Key.ItemCode,
													  Description = g.Key.Description,
													  OcrCode3 = g.Key.OcrCode3,
													  CardName = g.Key.CardName,
													  Fecha = g.Key.Fecha,
													  Documento = g.Key.Documento,
													  Quantity = g.Sum(x => x.Quantity),
													  TotalCosto = g.Sum(x => x.TotalCosto),
													  TotalVenta = g.Sum(x => x.TotalVenta),
													  Margen = g.Sum(x => x.Margen)
												  };

				List<Venta> listaFinalAgrupadaPorDetalle = new List<Venta>();

				//crear lista de tipo Venta, ya con los valores agrupados por detalle
				foreach (var itemVenta in resultadoAgrupadoPorDetalle.ToList())
				{
					var v = new Venta();
					v.CardName = itemVenta.CardName;
					v.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
					v.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
					v.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());
					if (v.TotalVenta != 0)
					{
						v.Margen = ((v.TotalVenta - v.TotalCosto) / v.TotalVenta) * 100;

					}
					else
					{
						v.Margen = -100;

					}
					v.Margen = Math.Round(decimal.Parse(v.Margen.ToString()), 2);

					decimal valor0_5 = 0.50M;
                    v.MargenEstandarizado = ((v.TotalVenta * v.Margen) / valor0_5) / 100;
					v.MargenEstandarizado = Math.Round(decimal.Parse(v.MargenEstandarizado.ToString()), 2);

					v.ItemCode = itemVenta.ItemCode;
					v.Description = itemVenta.Description;
					v.OcrCode3 = itemVenta.OcrCode3;

					try
					{
						v.VentaUnit = itemVenta.TotalVenta / itemVenta.Quantity;
						v.CostoUnit = itemVenta.TotalCosto / itemVenta.Quantity;
						v.Pvu = itemVenta.TotalVenta / itemVenta.Quantity;
					}
					catch (Exception e)
					{
						v.VentaUnit = 0;
						v.CostoUnit = 0;
						v.Pvu = 0;

					}

					v.SlpName = itemVenta.SlpName;
					v.Fecha = itemVenta.Fecha;
					v.Documento = itemVenta.Documento;

					v.VentaUnit = Math.Round(decimal.Parse(v.VentaUnit.ToString()), 2);
					v.CostoUnit = Math.Round(decimal.Parse(v.CostoUnit.ToString()), 2);
					v.Pvu = Math.Round(decimal.Parse(v.Pvu.ToString()), 2);


					listaFinalAgrupadaPorDetalle.Add(v);
				}

				Totales totalesPorDetalle = CalcularTotales(listaFinalAgrupadaPorDetalle);

				listaFinalAgrupadaPorDetalle = listaFinalAgrupadaPorDetalle.OrderByDescending(o => o.TotalVenta).ToList();


				ViewBag.resultadoPorDetalle = listaFinalAgrupadaPorDetalle;
				ViewBag.totalesPorDetalle = totalesPorDetalle;

				//esta variable de sesion se usará para el botón Exportar
				Session["listaVentasPorDetalle"] = listaFinalAgrupadaPorDetalle;

				//enviar filtros aplicados
				Filtro filtros = new Filtro();
				filtros.Sucursal = sucursal;
				filtros.Desde = desde.ToString("yyyy-MM-dd");
				filtros.Hasta = hasta.ToString("yyyy-MM-dd");
				filtros.Moneda = moneda;
				filtros.Divisionx = divisionx;
				filtros.Subdivisionx = subdivisionx;
				filtros.Lineax = lineax;
				filtros.CardCode = cardCode;
				filtros.ItemCode = null;
				filtros.SellerCode = null;
				filtros.Tab = "3";

				ViewBag.filtros = filtros;

				return PartialView();

			}

			return null;
		}

		public ActionResult Exportar()
		{
			Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];
			string rutaArchivoCompleta = "";
			if (objCurrentUser.Rol.id == 2)
			{
				rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_Vendedor.xlsx";
			}
			else
			{
				rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor.xlsx";
			}
			System.Diagnostics.Debug.Print("Paht " + rutaArchivoCompleta);

			SLDocument sl = new SLDocument(rutaArchivoCompleta);
			DateTime ahora = DateTime.Now;

			int filaInicial = 8;
			int colInicial = 1;
			int col = 0;

			List<Venta> ventas = new List<Venta>();
			var listaVentas = (List<Venta>)Session["listaVentas"];
			ventas.AddRange(listaVentas);

			int i = 1;
			foreach (var itemVenta in ventas)
			{
				col = 0;
				sl.SetCellValue(filaInicial, 1, i);
				// sl.SetCellValue(filaInicial, colInicial + col++, bool.Parse(itemVenta.SlpCode.ToString()));
				sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.SlpName);
				sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.Sucursal);
				sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.Quantity);
				sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.TotalVenta.ToString("N2"));
				if (objCurrentUser.Rol.id != 2)
				{
					sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.TotalCosto);
                    sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.Margen.ToString("N2") + "%");
                    sl.SetCellValue(filaInicial, colInicial + col++, itemVenta.MargenEstandarizado.ToString("N2"));
                }

				filaInicial++;
				i++;
			}

			Totales t = CalcularTotales(ventas);
			sl.SetCellValue(filaInicial, 3, "Totales");
			sl.SetCellValue(filaInicial, 4, t.TotalVentas.ToString("N2"));
			if (objCurrentUser.Rol.id != 2)
			{
				sl.SetCellValue(filaInicial, 5, t.TotalCompras.ToString("N2"));
                sl.SetCellValue(filaInicial, 6, t.TotalMargen.ToString("N2") + "%");
                sl.SetCellValue(filaInicial, 7, t.TotalMargenEstandarizado.ToString("N2"));
            }

			string pathMasUuid = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/" + Guid.NewGuid().ToString() + ".xlsx";
			sl.SaveAs(pathMasUuid);


			return File(pathMasUuid, "application/xls", "ReporteVentasVendedor_" +
				ahora.ToString("dd_MM_yyyy") + ".xlsx");



		}

		public ActionResult ExportarDetalle(string tab)
		{



			if (tab == "tab1")
			{
				Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];

				string rutaArchivoCompleta = "";

				if (objCurrentUser.Rol.id == 2) //rol 2 = vendedor
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_ProductoVendedor.xlsx";
				}
				else
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_Producto.xlsx";
				}
				System.Diagnostics.Debug.Print("Path " + rutaArchivoCompleta);


				SLDocument sl = new SLDocument(rutaArchivoCompleta);
				DateTime ahora = DateTime.Now;


				int filaInicial = 8;
				int colIni = 1;// 1= a, columna A
				int col = 0;



				List<Venta> ventas = new List<Venta>();

				var listaVentas = (List<Venta>)Session["listaVentasPorProducto"];
				ventas.AddRange(listaVentas);


				int i = 1;
				foreach (var itemVenta in ventas)
				{
					col = 0;
					sl.SetCellValue(filaInicial, 1, i);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.ItemCode);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Description);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.OcrCode3);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Quantity);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.VentaUnit.ToString("N2"));
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalVenta.ToString("N2"));
					if (objCurrentUser.Rol.id != 2)
					{
						sl.SetCellValue(filaInicial, colIni + col++, itemVenta.CostoUnit.ToString("N2"));
						sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalCosto.ToString("N2"));
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Margen.ToString("N2") + "%");
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.MargenEstandarizado.ToString("N2"));
                    }
					

					filaInicial++;
					i++;
				}

				Totales t = CalcularTotales(ventas);

				sl.SetCellValue(filaInicial, 5, "Totales");
				sl.SetCellValue(filaInicial, 6, t.TotalVentas.ToString("N2"));
				if (objCurrentUser.Rol.id != 2)
				{
					sl.SetCellValue(filaInicial, 7, t.TotalCompras.ToString("N2"));
                    sl.SetCellValue(filaInicial, 9, t.TotalMargen.ToString("N2") + "%");
                    sl.SetCellValue(filaInicial, 10, t.TotalMargenEstandarizado.ToString("N2"));
                }


				string pathMasUuid = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/" + Guid.NewGuid().ToString() + ".xlsx";

				sl.SaveAs(pathMasUuid);

				return File(pathMasUuid, "application/xlsx", "Ventas_" + ahora.ToString("dd_MM_yyy") + ".xlsx");
			}
			else
			if (tab == "tab2")
			{
				Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];

				string rutaArchivoCompleta = "";

				if (objCurrentUser.Rol.id == 2) //rol 2 = vendedor
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_ClienteVendedor.xlsx";
				}
				else
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_Cliente.xlsx";
				}
				System.Diagnostics.Debug.Print("Path " + rutaArchivoCompleta);


				SLDocument sl = new SLDocument(rutaArchivoCompleta);
				DateTime ahora = DateTime.Now;


				int filaInicial = 8;
				int colIni = 1;// 1= a, columna A
				int col = 0;



				List<Venta> ventas = new List<Venta>();

				var listaVentas = (List<Venta>)Session["listaVentasPorCliente"];
				ventas.AddRange(listaVentas);


				int i = 1;
				foreach (var itemVenta in ventas)
				{
					col = 0;
					sl.SetCellValue(filaInicial, 1, i);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.CardName);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Quantity);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalVenta.ToString("N2"));
					if (objCurrentUser.Rol.id != 2)
					{
						sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalCosto.ToString("N2"));
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Margen.ToString("N2") + "%");
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.MargenEstandarizado.ToString("N2"));
                    }

					filaInicial++;
					i++;
				}

				Totales t = CalcularTotales(ventas);

				sl.SetCellValue(filaInicial, 2, "Totales");
				sl.SetCellValue(filaInicial, 3, t.TotalVentas.ToString("N2"));
				if (objCurrentUser.Rol.id != 2)
				{
					sl.SetCellValue(filaInicial, 4, t.TotalCompras.ToString("N2"));
                    sl.SetCellValue(filaInicial, 5, t.TotalMargen.ToString("N2") + "%");
                    sl.SetCellValue(filaInicial, 6, t.TotalMargenEstandarizado.ToString("N2"));
                }

				string pathMasUuid = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/" + Guid.NewGuid().ToString() + ".xlsx";

				sl.SaveAs(pathMasUuid);

				return File(pathMasUuid, "application/xlsx", "Ventas_" + ahora.ToString("dd_MM_yyy") + ".xlsx");
			}
			else
			if (tab == "tab3")
			{
				Usuario objCurrentUser = (Usuario)Session[Constantes.SESSION_USUARIO];

				string rutaArchivoCompleta = "";

				if (objCurrentUser.Rol.id == 2) //rol 2 = vendedor
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_DetalleVendedor.xlsx";
				}
				else
				{
					rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/VentasVendedor_Detalle.xlsx";
				}
				System.Diagnostics.Debug.Print("Path " + rutaArchivoCompleta);


				SLDocument sl = new SLDocument(rutaArchivoCompleta);
				DateTime ahora = DateTime.Now;


				int filaInicial = 8;
				int colIni = 1;// 1= a, columna A
				int col = 0;



				List<Venta> ventas = new List<Venta>();

				var listaVentas = (List<Venta>)Session["listaVentasPorDetalle"];
				ventas.AddRange(listaVentas);


				int i = 1;
				foreach (var itemVenta in ventas)
				{
					col = 0;
					sl.SetCellValue(filaInicial, 1, i);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Fecha.ToString());
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Documento);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Description);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Quantity);
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Pvu.ToString("N2"));
					sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalVenta.ToString("N2"));
					if (objCurrentUser.Rol.id != 2)
					{
						sl.SetCellValue(filaInicial, colIni + col++, itemVenta.TotalCosto.ToString("N2"));
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.Margen.ToString("N2") + "%");
                        sl.SetCellValue(filaInicial, colIni + col++, itemVenta.MargenEstandarizado.ToString("N2"));
                    }
					
					filaInicial++;
					i++;
				}

				Totales t = CalcularTotales(ventas);

				sl.SetCellValue(filaInicial, 5, "Totales");
				sl.SetCellValue(filaInicial, 6, t.TotalVentas.ToString("N2"));
				if (objCurrentUser.Rol.id != 2)
				{
					sl.SetCellValue(filaInicial, 7, t.TotalCompras.ToString("N2"));
                    sl.SetCellValue(filaInicial, 8, t.TotalMargen.ToString("N2") + "%");
                    sl.SetCellValue(filaInicial, 9, t.TotalMargenEstandarizado.ToString("N2"));
                }

				string pathMasUuid = AppDomain.CurrentDomain.BaseDirectory + "/Views/VentasVendedor/" + Guid.NewGuid().ToString() + ".xlsx";

				sl.SaveAs(pathMasUuid);

				return File(pathMasUuid, "application/xlsx", "Ventas_" + ahora.ToString("dd_MM_yyy") + ".xlsx");
			}
			else
			{

				return null;
			}

		}


		List<Venta> ObtenerListaSinAgrupar(List<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasType> listaDatosSinAgruparDolares,
			List<MSS_REPORTES_TEST.services.ventas.ventasServices.ventasSolesType> listaDatosSinAgruparSoles,
			ICollection<Sucursal> sucList, ICollection<Division> divList, ICollection<SubDivision> subList, ICollection<Linea> linList, string selectedSucursalName)
		{

			bool sucOk = false;
			bool divOk = false;
			bool subDivOk = false;
			bool lineaOk = false;
			List<Venta> listaTmpSinAgrupar = new List<Venta>();


			//filtrar datos no validos para el filtro


			if (listaDatosSinAgruparDolares != null)//en dolares
			{
				Venta venta;
				foreach (var itemVenta in listaDatosSinAgruparDolares)
				{

					sucOk = false;
					divOk = false;
					subDivOk = false;
					lineaOk = false;

                    sucOk = selectedSucursalName != "Todas" ? new SucursalSeriesController().SucursalContainsSerie(selectedSucursalName, itemVenta.FolioPref) : true;
                    //foreach (var itemSuc in sucList)
                    //{
                    //	if (itemSuc.descripcion == itemVenta.Sucursal.ToString())
                    //	{
                    //		sucOk = true;
                    //	}
                    //}

                    foreach (var item in divList)
					{
						if (itemVenta.OcrCodeST != null && item.codigoSap == itemVenta.OcrCodeST.ToString())
						{
							divOk = true;
						}
					}

					foreach (var item in subList)
					{
						if (itemVenta.OcrCode2ST != null && item.codigoSap == itemVenta.OcrCode2ST.ToString())
						{
							subDivOk = true;
						}
					}


					foreach (var item in linList)
					{
						if (itemVenta.OcrCode3ST != null && item.codigoSap == itemVenta.OcrCode3ST.ToString())
						{
							lineaOk = true;
						}
					}


					if (sucOk && divOk && subDivOk && lineaOk)
					{

						venta = new Venta();
						venta.CardCode = itemVenta.CardCode;
						venta.CardName = itemVenta.CardName;
                        venta.Sucursal = new PhartecContext().SucursalSeries.Include("Sucursal").FirstOrDefault(x => x.Serie == itemVenta.FolioPref)?.Sucursal.descripcion;
                        venta.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
						venta.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
						venta.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());

						venta.ItemCode = itemVenta.ItemCode;
						venta.Description = itemVenta.Dscription;
						venta.OcrCode = itemVenta.OcrCodeST;
						venta.OcrCode3 = itemVenta.OcrCode3ST;
						venta.OcrCode2 = itemVenta.OcrCode2ST;
						venta.SlpCode = itemVenta.SlpCode;
						venta.SlpName = itemVenta.SlpName;
						venta.Fecha = itemVenta.Fecha;
						venta.Documento = itemVenta.Documento;
						


						listaTmpSinAgrupar.Add(venta);
					}
				}


			}
			else//en soles
			{
				Venta venta;
				foreach (var itemVenta in listaDatosSinAgruparSoles)
				{

					sucOk = false;
					divOk = false;
					subDivOk = false;
					lineaOk = false;

                    sucOk = selectedSucursalName != "Todas" ? new SucursalSeriesController().SucursalContainsSerie(selectedSucursalName, itemVenta.FolioPref) : true;

                    //foreach (var itemSuc in sucList)
                    //{
                    //	if (itemSuc.descripcion == itemVenta.Sucursal.ToString())
                    //	{
                    //		sucOk = true;
                    //	}
                    //}

                    foreach (var item in divList)
					{
						if (itemVenta.OcrCodeST != null && item.codigoSap == itemVenta.OcrCodeST.ToString())
						{
							divOk = true;
						}
					}

					foreach (var item in subList)
					{
						if (itemVenta.OcrCode2ST != null && item.codigoSap == itemVenta.OcrCode2ST.ToString())
						{
							subDivOk = true;
						}
					}


					foreach (var item in linList)
					{
						if (itemVenta.OcrCode3ST != null && item.codigoSap == itemVenta.OcrCode3ST.ToString())
						{
							lineaOk = true;
						}
					}


					if (sucOk && divOk && subDivOk && lineaOk)
					{
						venta = new Venta();
						venta.CardCode = itemVenta.CardCode;
						venta.CardName = itemVenta.CardName;
                        venta.Sucursal = new PhartecContext().SucursalSeries.Include("Sucursal").FirstOrDefault(x => x.Serie == itemVenta.FolioPref)?.Sucursal.descripcion;
                        venta.Quantity = decimal.Parse(itemVenta.Quantity.ToString());
						venta.TotalCosto = decimal.Parse(itemVenta.TotalCosto.ToString());
						venta.TotalVenta = decimal.Parse(itemVenta.TotalVenta.ToString());

						venta.ItemCode = itemVenta.ItemCode;
						venta.Description = itemVenta.Dscription;
						venta.OcrCode = itemVenta.OcrCodeST;
						venta.OcrCode3 = itemVenta.OcrCode3ST;
						venta.OcrCode2 = itemVenta.OcrCode2ST;
						venta.SlpCode = itemVenta.SlpCode;
						venta.SlpName = itemVenta.SlpName;
						venta.Fecha = itemVenta.Fecha;
						venta.Documento = itemVenta.Documento;



						listaTmpSinAgrupar.Add(venta);
					}
				}
			}

			return listaTmpSinAgrupar;

		}

	}


}

