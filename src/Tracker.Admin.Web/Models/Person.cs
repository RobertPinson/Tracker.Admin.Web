using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tracker.Admin.Web.Models
{
    public class Person : IEqualityComparer<Person>
    {
        public int Id { get; set; }

        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Display(Name="Last Name")]
        public string LastName { get; set; }

        [Display(Name="Photo")]
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
