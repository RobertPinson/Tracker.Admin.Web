using System.Collections.Generic;

namespace Tracker.Admin.Web.Models.LocationViewModels
{
    public class LocationPeopleViewModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public List<PersonInLocationViewModel> People { get; set; }
    }
}
