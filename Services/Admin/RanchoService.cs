using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class RanchoService
    {
        private readonly AppDbContext context;

        public RanchoService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Rancho> SelectRanchos()
        {
            List<Rancho> ranchoList;

            try
            {
                ranchoList = context.Ranchos.ToList();
            }
            catch
            {
                ranchoList = new List<Rancho>();
            }

            return ranchoList;
        }

        public Rancho SelectRancho(int IdRancho)
        {
            Rancho rancho;

            try
            {
                rancho = context.Ranchos.Find(IdRancho);
            }
            catch
            {
                rancho = new Rancho();
            }

            return rancho;
        }

        public int InsertRancho(Rancho rancho)
        {
            int res = 0;

            try
            {
                context.Ranchos.Add(rancho);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateRancho(Rancho rancho)
        {
            int res = 0;

            try
            {
                context.Ranchos.Update(rancho);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteRancho(int IdRancho)
        {
            int res = 0;

            try
            {
                Rancho rancho = context.Ranchos.Find(IdRancho);

                context.Ranchos.Remove(rancho);
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
