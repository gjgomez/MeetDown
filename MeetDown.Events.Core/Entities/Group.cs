using System;
using System.Collections.Generic;
using System.Text;

namespace MeetDown.Events.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public ICollection<Meet> Meets { get; set; }
    }
}
