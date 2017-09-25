using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetDown.Events.API
{
    public class EventsDataStore
    {
        public static EventsDataStore Current { get; } = new EventsDataStore();

        public IList<ViewModels.Groups.ReadViewModel> Groups { get; set; }

        public EventsDataStore()
        {
            Groups = new List<ViewModels.Groups.ReadViewModel>
            {
                new ViewModels.Groups.ReadViewModel
                {
                    Id = 1,
                    Name = ".NET Developers",
                    Description = "A group for people interested in .NET development.",
                    Location = "Chicago, IL",
                    Meets = new List<ViewModels.Meets.ReadViewModel> {
                        new ViewModels.Meets.ReadViewModel {
                            Id = 1,
                            Name = "Intro to C# ",
                            Description = "Overview of the C# language",
                            Location = "20 North Wacker Dr., Chicago, IL 60606",
                            DateOfMeet = DateTime.UtcNow.AddDays(10)
                        }
                    }
                },
                new ViewModels.Groups.ReadViewModel
                {
                    Id = 2,
                    Name = "Tennis League",
                    Description = "Tennis players of all ages and skill levels welcome.",
                    Location = "Chicago, IL",
                    Meets = new List<ViewModels.Meets.ReadViewModel> {
                        new ViewModels.Meets.ReadViewModel {
                            Id = 2,
                            Name = "Davis Cup Outing",
                            Description = "Join us to support the USA Davis Cup team against Slovakia",
                            Location = "Hoffman Estates, IL",
                            DateOfMeet = DateTime.UtcNow.AddYears(-3)
                        },
                        new ViewModels.Meets.ReadViewModel {
                            Id = 3,
                            Name = "Midwest Regional Tournament",
                            Description = "Level 5.0 regionals",
                            Location = "Northbrook, IL",
                            DateOfMeet = DateTime.UtcNow.AddMonths(2)
                        }
                    }
                }
            };
        }
    }
}
