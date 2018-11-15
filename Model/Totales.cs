using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Totales
    {
        private double totalVentas = 0;
        private double totalCompras = 0;
        private double totalMargen = 0;
        private double totalMargenEstandarizado = 0;

        

		public double TotalMargenEstandarizado { get => totalMargenEstandarizado; set => totalMargenEstandarizado = value; }
		public double TotalVentas { get => totalVentas; set => totalVentas = value; }
		public double TotalCompras { get => totalCompras; set => totalCompras = value; }
		public double TotalMargen { get => totalMargen; set => totalMargen = value; }
	}
}
