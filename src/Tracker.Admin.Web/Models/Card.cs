using System.Collections.Generic;

namespace Tracker.Admin.Web.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<PersonCard> PersonCards { get; set; }
        public ICollection<Movement> Movements { get; set; }
    }
}
