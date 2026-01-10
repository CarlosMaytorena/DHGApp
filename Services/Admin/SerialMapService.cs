using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;


namespace AgricolaDH_GApp.Services.Admin
{
    public class SerialMapService
    {
        private readonly AppDbContext context;

        public SerialMapService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<SerialMap> SelectSerials()
        {
            try { return context.SerialMap.ToList(); }
            catch { return new List<SerialMap>(); }
        }

        public SerialMap? SelectBySerial(string serialKey)
        {
            try { return context.SerialMap.Find(serialKey); }
            catch { return null; }
        }

        public int InsertSerial(SerialMap entity)
        {
            int res = 0;
            try
            {
                entity.SerialKey = (entity.SerialKey ?? "").ToUpperInvariant();
                if (entity.SerialKey.Length != 12) return -1;

                entity.ScrambledSerial = ScrambleSerial(entity.SerialKey);

                if (entity.CreatedAt == default) entity.CreatedAt = DateTime.UtcNow;

                context.SerialMap.Add(entity);
                context.SaveChanges();
            }
            catch
            {
                res = -1;
            }
            return res;
        }

        // Convenience overload
        public int InsertSerial(string serialKey, string orderNumber, string partNumber)
        {
            return InsertSerial(new SerialMap
            {
                SerialKey = serialKey,
                OrderNumber = orderNumber,
                PartNumber = partNumber
            });
        }

        public int DeleteSerial(string serialKey)
        {
            int res = 0;
            try
            {
                var row = context.SerialMap.Find(serialKey);
                if (row != null)
                {
                    context.SerialMap.Remove(row);
                    context.SaveChanges();
                }
            }
            catch { res = -1; }
            return res;
        }

        private string ScrambleSerial(string serial)
        {
            if (serial.Length != 12)
                return serial;

            return serial.Substring(6, 6) + serial.Substring(0, 6);
        }
    }
}