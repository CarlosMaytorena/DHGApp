using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class AreaService
    {
        private readonly AppDbContext context;

        public AreaService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Area> SelectAreas()
        {
            List<Area> areaList;

            try
            {
                areaList = context.Areas.OrderBy(a => a.Descripcion).ToList();
            }
            catch
            {
                areaList = new List<Area>();
            }

            return areaList;
        }

        public Area SelectArea(int IdArea)
        {
            Area area;

            try
            {
                area = context.Areas.Find(IdArea);
            }
            catch
            {
                area = new Area();
            }

            return area;
        }

        public int InsertArea(Area area)
        {
            int res = 0;

            try
            {
                context.Areas.Add(area);
                context.SaveChanges();
            }
            catch
            {
                res = -1;
            }

            return res;

        }

        public int UpdateArea(Area area)
        {
            int res = 0;

            try
            {
                context.Areas.Update(area);
                context.SaveChanges();
            }
            catch 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteArea(int IdArea)
        {
            int res = 0;

            try
            {
                Area area = context.Areas.Find(IdArea);

                context.Areas.Remove(area);
                context.SaveChanges();
            }
            catch
            {
                res = -1;
            }

            return res;
        }
    }
}
