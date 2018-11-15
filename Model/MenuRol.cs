using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("MenuRol")]
    public partial class MenuRol
    {
        public int id { get; set; }

        public bool accesa { get; set; }

        public bool registra { get; set; }

        public bool modifica { get; set; }

        public bool consulta { get; set; }

        public bool elimina { get; set; }

        public bool imprime { get; set; }

        public bool exporta { get; set; }

        public DateTime fecha_registro { get; set; }

        [NotMapped]
        public string Menu_descripcion { get; set; }

        [NotMapped]
        public int Menu_orden { get; set; }

        [Required]
        [StringLength(1)]
        public string estado { get; set; }

        public int Menu_id { get; set; }

        public int Rol_id { get; set; }

        public virtual Menu Menu { get; set; }

        public virtual Rol Rol { get; set; }

        //Métodos
        //Obtener la lista de menus xRol específico
        public List<MenuRol> ObtenerXRol(int rolId)
        {
            var lista = new List<MenuRol>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.MenuRol.Where(mxr => mxr.Rol_id == rolId && mxr.estado.Equals("A")).ToList();
                }
            }
            catch (Exception)
            {
                return lista;
            }

            return lista;
        }

        public MenuRol ObtenerXMenuYRol(int rolId, int menuId)
        {
            var result = new MenuRol();
            try
            {
                using (var ctx = new PhartecContext())
                {
                    result = ctx.MenuRol.Where(m => m.Rol_id == rolId && m.estado.Equals("A") && m.Menu_id == menuId).Single();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }
    }
}
