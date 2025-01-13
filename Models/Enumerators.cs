using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class RolEnumerators
    {
        public int Administrador = 1;
        public int Ingeniero = 2;
        public int Ingreso = 3;
        public int Almacen = 4;
        public int Egreso = 5;
        public int Contabilidad = 6;
    }

    public class OrdenDeCompraStatusEnumerators
    {
        public int Enviado = 1;
        public int Aceptado = 2;
        public int Rechazado = 3;
        public int PorIngresar = 4;
        public int Ingresado = 5;
        public int Cancelado = 6;
    }
}
