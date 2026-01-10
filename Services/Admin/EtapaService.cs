using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class EtapaService
    {
        private readonly AppDbContext context;

        public EtapaService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Etapa> SelectEtapas()
        {
            List<Etapa> etapaList;

            try
            {
                etapaList = context.Etapas.OrderBy(a => a.Descripcion).ToList();
            }
            catch
            {
                etapaList = new List<Etapa>();
            }

            return etapaList;
        }

        public Etapa SelectEtapa(int IdEtapa)
        {
            Etapa etapa;

            try
            {
                etapa = context.Etapas.Find(IdEtapa);
            }
            catch
            {
                etapa = new Etapa();
            }

            return etapa;
        }

        public int InsertEtapa(Etapa etapa)
        {
            int res = 0;

            try
            {
                context.Etapas.Add(etapa);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateEtapa(Etapa etapa)
        {
            int res = 0;

            try
            {
                context.Etapas.Update(etapa);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteEtapa(int IdEtapa)
        {
            int res = 0;

            try
            {
                Etapa etapa = context.Etapas.Find(IdEtapa);

                context.Etapas.Remove(etapa);
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
