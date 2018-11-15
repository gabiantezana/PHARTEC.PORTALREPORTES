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

    [Table("Usuario")]
    public partial class Usuario
    {
        public Usuario()
        {
            CentroCostoNivel = new List<CentroCostoNivel>();
            Solicitud = new List<Solicitud>();
            CentrosCosto = new List<CentroCosto>();
            Empresas = new List<Empresa>();
            //Agregar Sucursal
            Sucursales = new List<Sucursal>();
            //Agregar División
            Divisiones = new List<Division>();
            //Agregar SubDivision
            Subdivisiones = new List<SubDivision>();
            //Agregar Línea
            Lineas = new List<Linea>();
            articulosAutorizadosList = new List<string>();
            clientesAutorizadosList = new List<string>();
        }

        public int id { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Seleccione el rol del usuario")]
        public int Rol_id { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del usuario")]
        [StringLength(300)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar la cuenta del usuario")]
        [StringLength(100)]
        public string cuentaWeb { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña del usuario")]
        [StringLength(100)]
        public string passWeb { get; set; }

        [Required(ErrorMessage = "Debe ingresar el correo electrónico del usuario")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Ingrese un correo electrónico válido")]
        [StringLength(100)]
        public string correo { get; set; }

        [Required]
        [StringLength(1)]
        public string estado { get; set; }

        public DateTime fechaRegistro { get; set; }

        public int? codigo_sap { get; set; }

        [StringLength(100)]
        public string VCode { get; set; }


        [InverseProperty("Usuario")]
        public ICollection<CentroCostoNivel> CentroCostoNivel { get; set; }

        public virtual Rol Rol { get; set; }

        public ICollection<Solicitud> Solicitud { get; set; }

        [InverseProperty("Usuario")]
        public ICollection<CentroCosto> CentrosCosto { get; set; }

        public ICollection<Empresa> Empresas { get; set; }

        //Agregar Coleccion de Sucursales
        public ICollection<Sucursal> Sucursales { get; set; }

        //Agregar coleccion de Divisiones
        public ICollection<Division> Divisiones { get; set; }

        //Agregar coleccion de SubDivisiones
        public ICollection<SubDivision> Subdivisiones { get; set; }

        //Agregar colección de Líneas
        public ICollection<Linea> Lineas { get; set; }

        [NotMapped]
        public string Rol_Descripcion { get; set; }

        [NotMapped]
        public string Empresa_Descripcion { get; set; }

        [NotMapped]
        public string CentroCosto_Sap { get; set; }

        [NotMapped]
        public string fechaRegistroString { get; set; }

        [NotMapped]
        public string validacion { get; set; }

        [NotMapped]
        public ICollection<string> articulosAutorizadosList { get; set; }

        [NotMapped]
        public ICollection<string> clientesAutorizadosList { get; set; }

        //Métodos
        public List<Usuario> listar()
        {
            var lista = new List<Usuario>();
            try
            {
                using (var context = new PhartecContext())
                {
                    lista = context.Usuario.Include("Rol").ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public DbSet<Usuario> getObject()
        {
            using (var context = new PhartecContext())
            {
                return context.Usuario;
            }
        }

        public List<ComboModel> getForCombo()
        {
            var lista = new List<ComboModel>();
            try
            {
                using (var ctx = new PhartecContext())
                {
                    lista = ctx.Database.SqlQuery<ComboModel>("select id, nombre from Usuario where estado = @p0", "A").ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        //Obtener el nombre de un usuario por su ID
        public string getName(int id)
        {
            var result = String.Empty;

            try
            {
                using (var context = new PhartecContext())
                {
                    result = context.Usuario
                                     .Where(u => u.id == id)
                                     .Single().nombre;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }

        //Obtener el ID de un usuario por su email y usuario
        public int? getIDX(string email, string userWeb)
        {
            int? result = null;

            try
            {
                using (var context = new PhartecContext())
                {
                    if (context.Usuario.Where(u => u.correo.Equals(email) && u.cuentaWeb.Equals(userWeb)).Any())
                    {
                        result = context.Usuario
                                         .Where(u => u.correo.Equals(email) && u.cuentaWeb.Equals(userWeb))
                                         .Single().id;
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        //Obtener usuario por id
        public Usuario obtener(int id)
        {
            var usuario = new Usuario();
            try
            {
                using (var context = new PhartecContext())
                {
                    usuario = context.Usuario
                                     .Include("Rol")
                                     .Include("CentrosCosto")
                                     .Include("Empresas")
                                     .Include("Sucursales")
                                     .Include("Divisiones")
                                     .Include("Subdivisiones")
                                     .Include("Lineas")
                                     .Where(u => u.id == id)
                                     .Single();
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return usuario;
        }

        //Obtener usuario por código de usuario web
        public Usuario obtener(string cuentaWeb)
        {
            var usuario = new Usuario();
            try
            {
                using (var context = new PhartecContext())
                {
                    usuario = context.Usuario
                                     .Include("Rol")
                                     .Include("CentrosCosto")
                                     .Include("Empresas")
                                     .Include("Sucursales")
                                     .Include("Divisiones")
                                     .Include("Subdivisiones")
                                     .Include("Lineas")
                                     .Where(u => u.cuentaWeb == cuentaWeb)
                                     .Single();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return usuario;
        }

        //Métodos usados en el módulo de usuarios
        public string Guardar()
        {
            var result = string.Empty;

            try
            {
                using (var context = new PhartecContext())
                {
                    if (this.id == 0)
                    {
                        context.Entry(this).State = EntityState.Added;
                    }
                    else
                    {
                        #region Permiso Visualización de Artículos

                        //Desactivar todos los artículos permitidos para el usuario
                        context.UsuarioPermisoItems.Where(x => x.UsuarioId == this.id && x.TipoTablaSAP == 1).ToList().ForEach(x => x.PermisoVisualizacion = false);

                        //Activar los artículos permitidos
                        foreach (var item in this.articulosAutorizadosList)
                        {
                            var itemInDB = context.UsuarioPermisoItems.Where(x => x.UsuarioId == this.id && x.Code == item && x.TipoTablaSAP == 1).FirstOrDefault();
                            if (itemInDB == null)
                                context.UsuarioPermisoItems.Add(new UsuarioPermisoItems()
                                {
                                    Code = item,
                                    UsuarioId = this.id,
                                    PermisoVisualizacion = true,
                                    TipoTablaSAP = 1 //TODO: THIS SHOULD BE A CONSTANT!!!! x_x
                                });
                            else
                                itemInDB.PermisoVisualizacion = true;

                        }

                        #endregion

                        #region Permiso Visualización de Clientes

                        //Desactivar todos los clientes permitidos para el usuario
                        context.UsuarioPermisoItems.Where(x => x.UsuarioId == this.id && x.TipoTablaSAP == 2).ToList().ForEach(x => x.PermisoVisualizacion = false);

                        //Activar los clientes permitidos
                        foreach (var item in this.clientesAutorizadosList)
                        {
                            var itemInDB = context.UsuarioPermisoItems.Where(x => x.UsuarioId == this.id && x.Code == item && x.TipoTablaSAP == 2).FirstOrDefault();
                            if (itemInDB == null)
                                context.UsuarioPermisoItems.Add(new UsuarioPermisoItems()
                                {
                                    Code = item,
                                    UsuarioId = this.id,
                                    PermisoVisualizacion = true,
                                    TipoTablaSAP = 2 //TODO: THIS SHOULD BE A CONSTANT!!!! x_x
                                });
                            else
                                itemInDB.PermisoVisualizacion = true;
                        }

                        #endregion

                        //Eliminar los datos en la relación USUARIO-EMPRESA actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioEmpresa WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                        );
                        var empresaBK = this.Empresas;
                        this.Empresas = null;


                        //Eliminar los datos en relación USUARIO-CENTROCOSTO actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioCentroCosto WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                        );
                        var centrocostoBK = this.CentrosCosto;
                        this.CentrosCosto = null;


                        //Eliminar los datos en la relación USUARIO-SUCURSAL actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioSucursal WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                            );
                        var sucursalsBK = this.Sucursales;
                        this.Sucursales = null;

                        //Eliminar los datos en la relación USUARIO-DIVISION actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioDivision WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                            );
                        var divisionBK = this.Divisiones;
                        this.Divisiones = null;

                        //Eliminar los datos en la relación USUARIO-SUBDIVISION actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioSubDivision WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                            );
                        var subdivisionesBK = this.Subdivisiones;
                        this.Subdivisiones = null;

                        //Eliminar los datos en la relación USUARIO-SUCURSAL actual, para actualizar
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM UsuarioLinea WHERE Usuario_id = @id",
                            new SqlParameter("id", this.id)
                            );
                        var lineasBK = this.Lineas;
                        this.Lineas = null;

                        context.Entry(this).State = EntityState.Modified;
                        context.Entry(this).Property(u => u.passWeb).IsModified = false;
                        context.Entry(this).Property(u => u.VCode).IsModified = false;
                        this.CentrosCosto = centrocostoBK;
                        this.Empresas = empresaBK;
                        this.Sucursales = sucursalsBK;
                        this.Divisiones = divisionBK;
                        this.Subdivisiones = subdivisionesBK;
                        this.Lineas = lineasBK;
                    }


                    foreach (var e in this.Empresas)
                        context.Entry(e).State = EntityState.Unchanged;

                    foreach (var c in this.CentrosCosto)
                        context.Entry(c).State = EntityState.Unchanged;

                    foreach (var s in this.Sucursales)
                    {
                        context.Entry(s).State = EntityState.Unchanged;
                    }

                    foreach (var d in this.Divisiones)
                    {
                        context.Entry(d).State = EntityState.Unchanged;
                    }

                    foreach (var db in this.Subdivisiones)
                    {
                        context.Entry(db).State = EntityState.Unchanged;
                    }

                    foreach (var l in this.Lineas)
                    {
                        context.Entry(l).State = EntityState.Unchanged;
                    }
                    context.Entry(this.Rol).State = EntityState.Unchanged;
                    context.SaveChanges();
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
            catch (Exception e)
            {
                result = "DbException. " + e.Message;
            }

            return result;
        }

        public void Eliminar(int id)
        {
            try
            {
                using (var db = new PhartecContext())
                {
                    db.Entry(new Usuario { id = id }).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Validar los datos del usuario cuando se está logueando
        public string validar(string cuentaWeb, string password)
        {
            string res = string.Empty;
            try
            {
                using (var db = new PhartecContext())
                {
                    if (db.Usuario.Where(u => u.cuentaWeb.Equals(cuentaWeb)).Any())
                    {
                        if (!db.Usuario.Where(u => u.cuentaWeb.Equals(cuentaWeb) && u.passWeb.Equals(password)).Any())
                            res = "Contraseña incorrecta";
                        else if (db.Usuario.Where(u => u.cuentaWeb.Equals(cuentaWeb) && u.passWeb.Equals(password)).Single().estado.Equals("I"))
                            res = "Su cuenta se encuentra inactiva, contacte al administrador para poder ingresar.";
                        //else if(db.Usuario.Include("Rol").Where(u => u.cuentaWeb.Equals(cuentaWeb) && u.passWeb.Equals(password)).Single().Rol.)
                    }
                    else
                        res = "El usuario no existe";
                }
            }
            catch (Exception e)
            {
                res = e.Message;
            }

            return res;
        }

        //Validar existencia de usuario web
        public bool validateUserAccount()
        {
            var res = false;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    var founded = ctx.Usuario.Where(u => u.id != this.id && u.cuentaWeb.Equals(this.cuentaWeb)).Any();
                    if (founded)
                        res = true;
                }
            }
            catch (Exception)
            {
                return true;
            }

            return res;
        }

        //Validad existencia de código sap de usuario
        public bool validateCodSap()
        {
            var res = false;
            //int id = this.id;
            //int? codigosap = this.codigo_sap;

            //la
            try
            {
                using (var ctx = new PhartecContext())
                {
                    Usuario founded = ctx.Usuario.Where(x => x.id != this.id && x.codigo_sap == this.codigo_sap).FirstOrDefault();
                    if (founded != null)
                        res = true;
                }
            }
            catch (Exception e)
            {

                return true;
            }

            return res;
        }

        //Validar existencia de usuario web
        public bool validateUserAccount(string ctaWeb)
        {
            var res = false;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    var founded = ctx.Usuario.Where(u => u.id != this.id && u.cuentaWeb.Equals(ctaWeb)).Any();
                    if (founded)
                        res = true;
                }
            }
            catch (Exception)
            {
                return true;
            }

            return res;
        }



        //Método usado en el módulo mi perfil
        public void Actualizar()
        {
            using (var context = new PhartecContext())
            {
                context.Usuario.Attach(this);
                context.Entry(this).Property(u => u.nombre).IsModified = true;
                context.Entry(this).Property(u => u.cuentaWeb).IsModified = true;
                context.Entry(this).Property(u => u.correo).IsModified = true;
                context.SaveChanges();
            }
        }

        public void ActualizarPassword(int userId, string newPassword)
        {
            try
            {
                using (var ctx = new PhartecContext())
                {
                    ctx.Database.ExecuteSqlCommand(
                        "UPDATE Usuario SET passWeb = @pass where id = @id",
                        new SqlParameter("pass", newPassword),
                        new SqlParameter("id", userId)
                    );
                }
            }
            catch (Exception)
            {
            }
        }

        public class ComboModel
        {
            public int id { get; set; }
            public string nombre { get; set; }
        }


    }
}
