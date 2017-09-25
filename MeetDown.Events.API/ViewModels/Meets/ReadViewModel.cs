using System;

namespace MeetDown.Events.API.ViewModels.Meets
{
    public class ReadViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime DateOfMeet { get; set; }
    }
}
