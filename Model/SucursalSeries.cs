namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("SucursalSeries")]
    public partial class SucursalSeries
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Usuario(Id)
        /// </summary>
        [Required]
        public int SucursalId { get; set; }

        /// <summary>
        /// Identifica si el usuario tiene permiso de ver el artículo
        /// </summary>
        [Required]
        public string Serie { get; set; }

        /// <summary>
        /// OITM(ItemCode) *Tabla de SAP
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Identifica si el usuario tiene permiso de ver el artículo
        /// </summary>
        public bool Enable { get; set; }


        [ForeignKey("SucursalId")]
        public Sucursal Sucursal { get; set; }

        #region Business

        public bool IsValid()
        {
            var exists = new PhartecContext().SucursalSeries.Where(x => x.SucursalId == SucursalId && x.Serie == Serie && x.Id != Id).Count() > 0 ? true : false;
            return exists;
        }

        //Obtiene el listado de sucursales
        public List<SucursalSeries> GetList()
        {
            using (var db = new PhartecContext())
            {
                return db.SucursalSeries.Include("Sucursal").ToList();
            }
        }

        //Registra
        public string Save()
        {
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
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //Elimina
        public void Eliminar(int id)
        {
            using (var db = new PhartecContext())
            {
                db.Entry(new Sucursal { Id = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }


        #endregion
    }
}
