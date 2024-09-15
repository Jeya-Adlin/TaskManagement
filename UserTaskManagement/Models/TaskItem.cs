using System;
using System.ComponentModel.DataAnnotations;

namespace UserTaskManagement.Models
{
    // Models/TaskItem.cs
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum Status
    {
        ToDo,
        InProgress,
        Completed
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TaskName { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public Status Status { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

}
