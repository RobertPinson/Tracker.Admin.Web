﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Tracker.Admin.Web.Models;

namespace Tracker.Admin.Web.Data
{
    public static class TrackerStoreExtensions
    {
        public static void EnsureSampleData(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService(typeof(TrackerDbContext));
            var trackerContext = ((TrackerDbContext)context);

            if (!trackerContext.Card.Any())
            {
                trackerContext.Card.AddRange(new Card { Uid = "FD-A6-4A-95" }, new Card { Uid = "04-64-81-6A-D1-1E-80" }, new Card { Uid = "62-fb-37-45" }, new Card { Uid = "b4-b0-9d-fc" });
                trackerContext.Person.AddRange(new Person { FirstName = "Bill", LastName = "Gates" }, new Person { FirstName = "Joe", LastName = "Blogs" },new Person { FirstName = "Joe", LastName = "Blogs" },new Person { FirstName = "Joe", LastName = "Blogs" });
                trackerContext.PersonCard.AddRange(new PersonCard { CardId = 1, PersonId = 1 }, new PersonCard { CardId = 2, PersonId = 2 }, new PersonCard { CardId = 3, PersonId = 3 }, new PersonCard { CardId = 4, PersonId = 4 });
                trackerContext.SaveChanges();
            }

            if (!trackerContext.Device.Any())
            {
                if (!trackerContext.Location.Any(l => l.Name == "OffSite"))
                {
                    var offSite = new Location { Name = "OffSite" };
                    trackerContext.Location.Add(offSite);
                    var headOffice = new Location { Name = "Head Office" };
                    trackerContext.Location.Add(headOffice);
                    trackerContext.SaveChanges();

                    trackerContext.Device.AddRange(
                        new Device { Name = "Main Entrance", IsActive = true, LocationId = headOffice.Id },
                        new Device { Name = "Side Entrance", IsActive = true, LocationId = headOffice.Id });
                    trackerContext.SaveChanges();
                }
            }
        }
    }
}