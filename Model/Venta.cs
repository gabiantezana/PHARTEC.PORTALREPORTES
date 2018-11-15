using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{

	public class Venta
	{
		private string cardCode;
		private string cardName;
		private string sucursal;
		private decimal quantity;
		private decimal totalCosto;
		private decimal totalVenta;
		private decimal margen;
		private decimal margenEstandarizado;

		//
		private string itemCode;
		private string description;
		private string ocrCode;
		private string ocrCode3;
		private string ocrCode2;//subdivision
		private decimal ventaUnit;
		private decimal costoUnit;
		private int? slpCode;
		private string slpName;//seller
		private DateTime? fecha;
		private string documento;
		private decimal pvu;

		private int? docEntry;
		private int? lineNum;


		public decimal Pvu
		{
			get { return pvu; }
			set { pvu = value; }
		}

		public string Documento
		{
			get { return documento; }
			set { documento = value; }
		}

		public DateTime? Fecha
		{
			get { return fecha; }
			set { fecha = value; }
		}

		public string SlpName
		{
			get { return slpName; }
			set { slpName = value; }
		}

		public int? SlpCode
		{
			get { return slpCode; }
			set { slpCode = value; }
		}

		public decimal CostoUnit
		{
			get { return costoUnit; }
			set { costoUnit = value; }
		}

		public decimal VentaUnit
		{
			get { return ventaUnit; }
			set { ventaUnit = value; }
		}

		public string OcrCode3
		{
			get { return ocrCode3; }
			set { ocrCode3 = value; }
		}


		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public string ItemCode
		{
			get { return itemCode; }
			set { itemCode = value; }
		}


		public string CardCode
		{
			get { return cardCode; }
			set { cardCode = value; }
		}

		public string CardName
		{
			get { return cardName; }
			set { cardName = value; }
		}

		public string Sucursal
		{
			get { return sucursal; }
			set { sucursal = value; }
		}

		public decimal Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public decimal TotalCosto
		{
			get { return totalCosto; }
			set { totalCosto = value; }
		}

		public decimal TotalVenta
		{
			get { return totalVenta; }
			set { totalVenta = value; }
		}

		public decimal Margen
		{
			get { return margen; }
			set { margen = value; }
		}

		public decimal MargenEstandarizado { get => margenEstandarizado; set => margenEstandarizado = value; }
		public int? DocEntry { get => docEntry; set => docEntry = value; }
		public int? LineNum { get => lineNum; set => lineNum = value; }
		public string OcrCode2 { get => ocrCode2; set => ocrCode2 = value; }
		public string OcrCode { get => ocrCode; set => ocrCode = value; }
	}

}
