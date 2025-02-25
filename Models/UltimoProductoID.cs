using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class UltimoProductoID
    {
        public int ID { get; set; }

        public int LastBarcodeNumber { get; set; }


    }
}
