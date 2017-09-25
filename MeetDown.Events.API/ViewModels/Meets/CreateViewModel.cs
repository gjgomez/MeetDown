using System;
using System.ComponentModel.DataAnnotations;

namespace MeetDown.Events.API.ViewModels.Meets
{
    public class CreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime DateOfMeet { get; set; }
    }
}
