using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class VentaProducto
    {
        public string ItemCode { get; set; }
        public string Producto { get; set; }
        public string Sucursal { get; set; }
        public decimal Quantity { get; set; }
        public decimal VentaUnitaria { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal TotalCosto { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal Margen { get; set; }
    }
}
