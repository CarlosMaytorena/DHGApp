using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public static class RolEnumerators
    {
        public const int Administrador = 1;
        public const int Ingeniero = 2;
        public const int Ingreso = 3;
        public const int Almacen = 4;
        public const int Egreso = 5;
        public const int Contabilidad = 6;
    }

    public static class OrdenDeCompraStatusEnumerators
    {
        public const int Enviado = 1;
        public const int Aceptado = 2;
        public const int Rechazado = 3;
        public const int PorIngresar = 4;
        public const int Ingresado = 5;
        public const int Cancelado = 6;
    }
}
