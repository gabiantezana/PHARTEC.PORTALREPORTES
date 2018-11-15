namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Data.SqlClient;
    using System.Linq;

    [Table("Empresa")]
    public partial class Empresa
    {
        public Empresa()
        {
            CentroCosto = new List<CentroCosto>();
            Usuario = new List<Usuario>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }

        [StringLength(300)]
        public string db_name { get; set; }

        [StringLength(100)]
        public string usuario { get; set; }

        [StringLength(100)]
        public string password { get; set; }

        [StringLength(1)]
        public string estado { get; set; }

        public string validacion_sl { get; set; }

        [InverseProperty("Empresa")]
        public ICollection<CentroCosto> CentroCosto { get; set; }

        public ICollection<Usuario> Usuario { get; set; }


        //Métodos
        public List<Empresa> listar()
        {
            var lista = new List<Empresa>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.Empresa.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public int obtenerCodigoXDescripcion(string descripcion)
        {
            int EmpresaId = -1;

            try
            {
                using (var db = new PhartecContext())
                {
                    if (db.Empresa.Where(e => e.descripcion.ToUpper().Trim().Equals(descripcion.ToUpper().Trim())).Any())
                        EmpresaId = db.Empresa.Where(e => e.descripcion.ToUpper().Trim().Equals(descripcion.ToUpper().Trim())).Single().id;
                    else
                        EmpresaId = -1;
                }
            }
            catch (Exception)
            {
                EmpresaId = -1;
            }

            return EmpresaId;
        }

        public JqGridModel<Empresa> listar(JqGrid jq)
        {
            JqGridModel<Empresa> jqm = new JqGridModel<Empresa>();

            using (var ctx = new PhartecContext())
            {
                // Traemos la cantidad de registros
                jq.count = ctx.Menu.Count();

                // Configuramos el JqGridModel
                jqm.Config(jq);

                jqm.DataSource(ctx.Database.SqlQuery<Empresa>("select id, descripcion, db_name, usuario, password,estado, validacion_sl from Empresa order by "
                                                                + jqm.sord + " OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY;",
                        new SqlParameter("OFFSET", jqm.start),
                        new SqlParameter("FETCH", jqm.limit)).ToList());

            }

            return jqm;
        }

        public string Guardar()
        {
            string resp = string.Empty;

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
                        context.Entry(this).State = EntityState.Modified;
                        context.Entry(this).Property(e => e.validacion_sl).IsModified = false;
                    }

                    foreach (var e in this.CentroCosto)
                        context.Entry(e).State = EntityState.Unchanged;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                resp = ex.Message;
            }

            return resp;
        }

        public string Eliminar()
        {
            string result = string.Empty;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    ctx.Database.ExecuteSqlCommand(
                                            "UPDATE Empresa SET estado = 'I' where id = @id",
                                            new SqlParameter("id", this.id)
                                        );
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        //Comprobar el estado de una empresa
        public bool isActive(int empID)
        {
            var res = true;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    var state = ctx.Empresa.Where(e => e.id == empID).Single().estado;
                    if (state.Equals("I"))
                        res = false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return res;
        }
    }
}
