using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracker.Admin.Web.Dtos
{
    public class MovementDto
    {
        public string CardUid { get; set; }
        public int DeviceId { get; set; }
        public DateTime SwipeTime { get; set; }
    }

    public class MovementResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public bool Ingress { get; set; }
        public string CardUid { get; set; }
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }
}
