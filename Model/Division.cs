namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Data.Entity;
    using System.Text;

    [Table("Division")]
    public partial class Division
    {
     
        public Division()
        {
            Usuario = new List<Usuario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }

        [Required]
        [StringLength(40)]
        public string codigoSap { get; set; }

      
        public ICollection<Usuario> Usuario { get; set; }

        //LISTADO
        public List<Division> Listar() {

            var lista = new List<Division>();

            try
            {
                using (var db = new PhartecContext()) {

                    lista = db.Division.ToList();
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
            return lista;
        }

        //REGISTRAR

        public string Guardar() {

            var res = String.Empty;
            try
            {
                using (var db = new PhartecContext()) {
                    if (this.Id == 0)
                        db.Entry(this).State = EntityState.Added;
                    else
                        db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        //Eliminar

        public void Eliminar(int id) {
            try
            {
                using (var db = new PhartecContext()) {
                    db.Entry(new Division { Id = id }).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                    var encontrado = db.Division.Where(s => s.Id != this.Id && s.codigoSap.Equals(this.codigoSap)).Any();
                    if (encontrado)
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return res;
        }

    }
}
