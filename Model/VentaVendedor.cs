using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class VentaVendedor
    {
        private int? slpCode;
        private string slpName;
        private string sucursal;
        private decimal quantity;
        private decimal totalCosto;
        private decimal totalVenta;
        private decimal margen;

        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string OcrCode3 { get; set; }
        public decimal VentaUnit { get; set; }
        public decimal CostoUnit { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime? Fecha { get; set; }
        public string Documento { get; set; }
        public decimal Pvu { get; set; }


        public int? SlpCode
        {
            get { return slpCode; }
            set { slpCode = value; }
        }

        public string SlpName
        {
            get { return slpName; }
            set { slpName = value; }
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
    }
}
