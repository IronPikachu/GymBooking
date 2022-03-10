using Microsoft.AspNetCore.Identity;

namespace GymBooking.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<ApplicationUserGymClass> AttendingClasses { get; set; }

    //public string Name { get; set; }
    //public DateTime CreatedDate { get; set; }
}
