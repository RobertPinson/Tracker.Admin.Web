using Tracker.Admin.Web.Domain.Model;
using Tracker.Admin.Web.Dtos;

namespace Tracker.Admin.Web.Domain.Services
{
    public interface IMovementService
    {
        MovementResult Save(MovementDto movement);
    }
}
