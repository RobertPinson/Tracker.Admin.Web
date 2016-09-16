using System.Collections.Generic;

namespace Tracker.Admin.Web.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Movement> Movements { get; set; }
        public ICollection<Device> Devices { get; set; }
    }
}