using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    public class IAuditable
    {
        [NotMapped]
        public int usuario_curr { get; set; }

        public int? CreadoPor { get; set; }
        public DateTime? Creado { get; set; }
        public int? ActualizadoPor { get; set; }
        public DateTime? Actualizado { get; set; }
    }
}
