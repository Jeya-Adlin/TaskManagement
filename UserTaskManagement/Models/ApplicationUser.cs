using Microsoft.AspNetCore.Identity;
namespace UserTaskManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<TaskItem> TasksList { get; set; }
    }
}
