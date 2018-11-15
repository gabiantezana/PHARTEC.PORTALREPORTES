using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class VentaVendedorDetalle
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string OcrCode3 { get; set; }
        public decimal VentaUnit { get; set; }
        public decimal CostoUnit { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime? Fecha { get; set; }
        public string Documento { get; set; }
        public decimal PVU { get; set; }
    }
}
