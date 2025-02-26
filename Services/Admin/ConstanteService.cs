using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class ConstanteService
    {
        private readonly AppDbContext context;

        public ConstanteService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Constante> SelectConstantes()
        {
            List<Constante> constanteList;

            try
            {
                constanteList = context.Constantes.ToList();
            }
            catch
            {
                constanteList = new List<Constante>();
            }

            return constanteList;
        }

        public Constante SelectConstante(string Descripcion)
        {
            Constante constante;

            try
            {
                constante = context.Constantes.ToList().Where(m => m.Descripcion == Descripcion).FirstOrDefault();
            }
            catch
            {
                constante = new Constante();
            }

            return constante;
        }
        
    }
}
