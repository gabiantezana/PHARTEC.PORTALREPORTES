using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("Producto")]
    public partial class Producto
    {
        public int id { get; set; }

        [StringLength(10)]
        public string name { get; set; }

        public decimal? precio { get; set; }

        public List<Producto> listar()
        {
            var lista = new List<Producto>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.Producto.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }
    }


}
