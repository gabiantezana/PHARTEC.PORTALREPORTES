using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Filtro
    {
        public string Sucursal { get; set; }
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public string Moneda { get; set; }
        public string Divisionx { get; set; }
        public string Subdivisionx { get; set; }
        public string Lineax { get; set; }
        public string ItemCode { get; set; } //codigo prod
        public string CardCode { get; set; } //codigo registro
        public string SellerCode { get; set; } //codigo prod
        public string Tab { get; set; }
        public string SlpCode { get; set; }
    }
}
