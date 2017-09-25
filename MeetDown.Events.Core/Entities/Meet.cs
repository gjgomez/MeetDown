using System;
using System.Collections.Generic;
using System.Text;

namespace MeetDown.Events.Core.Entities
{
    public class Meet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime DateOfMeet { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }
    }
}
