using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace AgricolaDH_GApp.Models
{
    public class Factura
    {
        public double Cantidad { get; set; }
        public double ClaveProdServ {  get; set; }
        public string CLaveUnidad { get; set; }
        public string Descripcion { get; set; }
        public double Importe { get; set; }
        public int NoIdentificacion { get; set; }

    }

}
