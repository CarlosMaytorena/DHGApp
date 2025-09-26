using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class SerialMap
    {
        // PK = the 12-char serial you generate client-side
        public string SerialKey { get; set; } = default!;

        public string OrderNumber { get; set; } = default!;
        public string PartNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
