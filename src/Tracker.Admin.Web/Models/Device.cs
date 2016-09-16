using System.Collections.Generic;

namespace Tracker.Admin.Web.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public bool IsActive { get; set; }

        public Location Location { get; set; }
        public ICollection<Movement> Movements { get; set; }
    }
}