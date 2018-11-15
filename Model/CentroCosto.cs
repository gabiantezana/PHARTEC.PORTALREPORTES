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

    [Table("CentroCosto")]
    public partial class CentroCosto
    {
        public CentroCosto()
        {
            CentroCostoNivel = new List<CentroCostoNivel>();
            Usuario = new List<Usuario>();
        }

        public int id { get; set; }

        public int Empresa_id { get; set; }

        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }

        public string codigoSap { get; set; }

        [StringLength(1)]
        public string estado { get; set; }

        [InverseProperty("CentroCosto")]
        public ICollection<CentroCostoNivel> CentroCostoNivel { get; set; }

        [InverseProperty("CentroCosto")]
        public virtual Empresa Empresa { get; set; }

        [InverseProperty("CentrosCosto")]
        public ICollection<Usuario> Usuario { get; set; }

        //Métodos
        public List<CentroCosto> listar(int EmpresaId)
        {
            var lista = new List<CentroCosto>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.CentroCosto.Where(c => c.Empresa_id == EmpresaId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public List<CentroCosto> listar()
        {
            var lista = new List<CentroCosto>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.CentroCosto
                                    .Include("Empresa")
                                    .Include("CentroCostoNivel").ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public List<CentroCosto> listarXEmpresa(int id)
        {
            var lista = new List<CentroCosto>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.CentroCosto.Include("Empresa").Where(c => c.Empresa_id == id).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        //Obtener centro de costo por id
        public CentroCosto obtener(int id)
        {
            var cCosto = new CentroCosto();
            try
            {
                using (var context = new PhartecContext())
                {
                    cCosto = context.CentroCosto
                                     .Include("Empresa")
                                     .Include("CentroCostoNivel")
                                     .Where(u => u.id == id)
                                     .Single();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return cCosto;
        }

        public int obtenerCodigoXCodSAP(string codSAP, int empresaID)
        {
            int idCentro = -1;
            try
            {
                using (var ctx = new PhartecContext())
                {
                    if (ctx.CentroCosto.Where(c => c.codigoSap.ToUpper().Trim().Equals(codSAP.ToUpper().Trim()) && c.Empresa_id == empresaID).Any())
                        idCentro = ctx.CentroCosto.Where(c => c.codigoSap.ToUpper().Trim().Equals(codSAP.ToUpper().Trim()) && c.Empresa_id == empresaID).Single().id;
                    else
                        idCentro = -1;
                }
            }
            catch (Exception)
            {
                idCentro = -1;   
            }

            return idCentro;
        }

        public string Guardar()
        {
            var res = String.Empty;
            
            try
            {
                using (var context = new PhartecContext())
                {
                    if (this.id == 0)
                    {
                        context.Entry(this).State = EntityState.Added;
                        context.SaveChanges();
                    }
                    else
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            context.Database.ExecuteSqlCommand(
                            "DELETE FROM CentroCostoNivel WHERE CentroCosto_id = @id",
                            new SqlParameter("id", this.id)
                            );

                            var centrocostoBK = this.CentroCostoNivel;
                            this.CentroCostoNivel = null;

                            context.Entry(this).State = EntityState.Modified;
                            this.CentroCostoNivel = centrocostoBK;


                            try
                            {
                                context.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                res = e.Message;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                res = e.Message;
            }

            return res;
        }

        //Util Model
        public class ListModel
        {
            public int id { get; set; }
            public string descripcion { get; set; }
            public string empresa { get; set; }
        }
    
        //Validar si un centro de costo se encuentra activo
        public bool isActive(int ccID)
        {
            var res = true;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    var state = ctx.CentroCosto.Where(c => c.id == ccID).Single().estado;
                    if (!state.Equals("A"))
                        res = false;
                }
            }
            catch (Exception)
            {
                res = false;   
            }

            return res;
        }
    }
}
