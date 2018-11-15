using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Model
{

    [Table("Menu")]
    public partial class Menu
    {
        public Menu()
        {
            MenuRol = new HashSet<MenuRol>();
        }

        public int id { get; set; }

        [StringLength(200)]
        public string descripcion { get; set; }

        [StringLength(200)]
        public string actionName { get; set; }

        [StringLength(200)]
        public string controllerName { get; set; }

        [StringLength(200)]
        public string iconName { get; set; }

        public int? orden { get; set; }

        public ICollection<MenuRol> MenuRol { get; set; }

        //funciones propias
        public List<Menu> listar()
        {
            var result = new List<Menu>();

            try
            {
                using (var db = new PhartecContext())
                {
                    result = db.Menu.OrderBy(m => m.orden).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return result;
        }

        public JqGridModel<Menu> listar(JqGrid jq)
        {
            JqGridModel<Menu> jqm = new JqGridModel<Menu>();

            using (var ctx = new PhartecContext())
            {
                // Traemos la cantidad de registros
                jq.count = ctx.Menu.Count();

                // Configuramos el JqGridModel
                jqm.Config(jq);

                jqm.DataSource(ctx.Database.SqlQuery<Menu>("select id, descripcion, actionName, controllerName, iconName, orden from Menu order by "
                                                                + jqm.sord + " OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY;",
                        new SqlParameter("OFFSET", jqm.start),
                        new SqlParameter("FETCH", jqm.limit)).ToList());

            }

            return jqm;
        }

        public int obtenerIdFromController(string controllerName)
        {
            var res = 0;
            try
            {
                using (var ctx = new PhartecContext())
                {
                    res = ctx.Menu.Where(m => m.controllerName.Equals(controllerName)).Single().id;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return res; 
        }

        public void Guardar()
        {
            using (var context = new PhartecContext())
            {
                if (this.id == 0)
                {
                    context.Entry(this).State = EntityState.Added;
                }
                else
                {
                    context.Entry(this).State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }

        public void Eliminar()
        {
            using (var context = new PhartecContext())
            {
                context.Entry(this).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}
