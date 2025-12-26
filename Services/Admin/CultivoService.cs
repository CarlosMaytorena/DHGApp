using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class CultivoService
    {
        private readonly AppDbContext context;

        public CultivoService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Cultivo> SelectCultivos()
        {
            List<Cultivo> cultivoList;

            try
            {
                cultivoList = context.Cultivos.OrderBy(a => a.Descripcion).ToList();
            }
            catch
            {
                cultivoList = new List<Cultivo>();
            }

            return cultivoList;
        }

        public Cultivo SelectCultivo(int IdCultivo)
        {
            Cultivo cultivo;

            try
            {
                cultivo = context.Cultivos.Find(IdCultivo);
            }
            catch
            {
                cultivo = new Cultivo();
            }

            return cultivo;
        }

        public int InsertCultivo(Cultivo cultivo)
        {
            int res = 0;

            try
            {
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateCultivo(Cultivo cultivo)
        {
            int res = 0;

            try
            {
                context.Cultivos.Update(cultivo);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteCultivo(int IdCultivo)
        {
            int res = 0;

            try
            {
                Cultivo cultivo = context.Cultivos.Find(IdCultivo);

                context.Cultivos.Remove(cultivo);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }
    }
}
