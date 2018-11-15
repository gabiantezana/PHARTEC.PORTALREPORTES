using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Moneda
    {
        public string valor { get; set; }
        public string descripcion { get; set; }

        public List<Moneda> listar() {
            var lista = new List<Moneda>();

            lista.Add(new Moneda { valor = "USD", descripcion = "Dólares" });
            lista.Add(new Moneda { valor = "SOL", descripcion = "Soles" });
            return lista;
        }
    }
}
