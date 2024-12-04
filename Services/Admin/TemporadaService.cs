using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class TemporadaService
    {
        private readonly AppDbContext context;

        public TemporadaService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Temporada> SelectTemporadas()
        {
            List<Temporada> temporadaList;

            try
            {
                temporadaList = context.Temporadas.ToList();
            }
            catch
            {
                temporadaList = new List<Temporada>();
            }

            return temporadaList;
        }

        public Temporada SelectTemporada(int IdTemporada)
        {
            Temporada temporada;

            try
            {
                temporada = context.Temporadas.Find(IdTemporada);
            }
            catch
            {
                temporada = new Temporada();
            }

            return temporada;
        }

        public int InsertTemporada(Temporada temporada)
        {
            int res = 0;

            try
            {
                context.Temporadas.Add(temporada);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateTemporada(Temporada temporada)
        {
            int res = 0;

            try
            {
                context.Temporadas.Update(temporada);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteTemporada(int IdTemporada)
        {
            int res = 0;

            try
            {
                Temporada temporada = context.Temporadas.Find(IdTemporada);

                context.Temporadas.Remove(temporada);
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
