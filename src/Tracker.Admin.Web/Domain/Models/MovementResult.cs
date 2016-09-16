using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Admin.Web.Models;

namespace Tracker.Admin.Web.Domain.Model
{
    public class MovementResult
    {
        public Person Person { get; set; }
        public bool Ingress { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string CardUid { get; set; }
    }
}
