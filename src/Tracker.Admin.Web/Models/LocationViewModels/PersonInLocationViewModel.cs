using System;

namespace Tracker.Admin.Web.Models.LocationViewModels
{
    public class PersonInLocationViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CardUid { get; set; }
        public DateTime SwipeTime { get; set; }
    }
}