namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Text;


    [Table("Sucursal")]
    public partial class Sucursal
    {

        public Sucursal()
        {
            Usuario = new List<Usuario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }

        [StringLength(40)]
        public string codigoSap { get; set; }


        public ICollection<Usuario> Usuario { get; set; }


        // public ICollection<Usuario> Usuario { get; set; }
        //[StringLength(1)]
        //public string estado { get; set; }

        //LISTADO
        public List<Sucursal> listar()
        {
            var lista = new List<Sucursal>();
            try
            {
                using (var db = new PhartecContext())
                {
                    lista = db.Sucursal.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
            return lista;
        }

        //REGISTRAR
        public string Guardar()
        {
            var res = String.Empty;
            try
            {
                using (var context = new PhartecContext())
                {
                    if (this.Id == 0)
                        context.Entry(this).State = EntityState.Added;
                    else
                        context.Entry(this).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        //ELIMINAR
        public void Eliminar(int id)
        {
            try
            {
                using (var db = new PhartecContext())
                {
                    db.Entry(new Sucursal { Id = id }).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

       // Valida la existencia del cod sap
            public bool ValidateCodSap()
            {
                var res = false;
                try
                {
                    using (var db = new PhartecContext())
                    {
                        var encontrado = db.Sucursal.Where(s => s.Id != this.Id && s.codigoSap.Equals(this.codigoSap)).Any();
                        if (encontrado)
                        {
                            res = true;
                        }
                    }
                }
                catch (Exception ex )
                {
                    throw new Exception(ex.Message);
                }
                return res;
            }

    }
}
