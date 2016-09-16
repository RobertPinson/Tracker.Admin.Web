using System.Collections;
using System.Collections.Generic;

namespace Tracker.Admin.Web.Models
{
    public class Person : IEqualityComparer<Person>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Image { get; set; }

        public ICollection<PersonCard> PersonCards { get; set; }
        public bool Equals(Person x, Person y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Person obj)
        {
            return obj.Id;
        }
    }
}
