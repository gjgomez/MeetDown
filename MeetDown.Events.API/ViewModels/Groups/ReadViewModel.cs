using System.Collections.Generic;

namespace MeetDown.Events.API.ViewModels.Groups
{
    public class ReadViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public IList<Meets.ReadViewModel> Meets { get; set; } = new List<Meets.ReadViewModel>();
    }
}
