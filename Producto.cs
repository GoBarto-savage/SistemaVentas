using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas
{
    public class Producto
    {
        public long ID { get; set; }
        public String Nombre { get; set; }
        public String Descripcion { get; set; }
        public String PrecioCompra { get; set; }
        public String PrecioVenta { get; set; }
        public int Stock { get; set; }
        public String FechaHora { get; set; }

       
    }
}
