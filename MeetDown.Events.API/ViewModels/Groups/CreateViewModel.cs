using System;
using System.ComponentModel.DataAnnotations;

namespace MeetDown.Events.API.ViewModels.Groups
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
    }
}
