namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;
    using System.Linq;

    [Table("Rol")]
    public partial class Rol
    {
        public Rol()
        {
            MenuRol = new HashSet<MenuRol>();
            Usuario = new HashSet<Usuario>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }

        public DateTime? fechaRegistro { get; set; }

        public ICollection<MenuRol> MenuRol { get; set; }

        public ICollection<Usuario> Usuario { get; set; }


        //Métodos
        public List<Rol> listar()
        {
            var lista = new List<Rol>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.Rol.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public Rol obtener(int id)
        {
            var rol = new Rol();
            try
            {
                using (var db = new PhartecContext())
                {
                    rol = db.Rol.Include("MenuRol").Where(r => r.id == id).Single();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return rol;
        }

        public int obtenerIdXDescripcion(string desRol)
        {
            int idRol = -1;
            try
            {
                using (var db = new PhartecContext())
                {
                    if (db.Rol.Where(r => r.descripcion.ToUpper().Trim().Equals(desRol.Trim())).Any())
                        idRol = db.Rol.Where(r => r.descripcion.ToUpper().Trim().Equals(desRol.Trim())).Single().id;
                    else
                        idRol = -1;
                }
            }
            catch (Exception)
            {
                idRol = -1;
            }

            return idRol;
        }

        public Rol obtenerSingle(int id)
        {
            var rol = new Rol();
            try
            {
                using (var db = new PhartecContext())
                {
                    rol = db.Rol.Where(r => r.id == id).Single();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return rol;
        }

        public string Guardar()
        {
            var result = string.Empty;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    if (this.id == 0)
                    {
                        ctx.Entry(this).State = EntityState.Added;
                    }
                    else
                    {
                        //Actualización
                        //Eliminar los datos en la relación USUARIO-EMPRESA actual, para actualizar
                        ctx.Database.ExecuteSqlCommand(
                            "DELETE FROM MenuRol WHERE Rol_id = @id",
                            new SqlParameter("id", this.id)
                        );
                        var mxrolBK = this.MenuRol;
                        this.MenuRol = null;

                        ctx.Entry(this).State = EntityState.Modified;
                        this.MenuRol = mxrolBK;
                    }

                    foreach (var m in this.MenuRol)
                        ctx.Entry(m.Menu).State = EntityState.Unchanged;

                    ctx.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                string errors = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    errors += "Type " + eve.Entry.Entity.GetType().Name + " has the following vaidation errors: " + eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errors += " Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;
                    }
                }

                result = "Entity errors. " + errors;
            }
            catch (Exception ex)
            {
                result = "DBError. " + ex.Message;
            }

            return result;
        }

    }
}
