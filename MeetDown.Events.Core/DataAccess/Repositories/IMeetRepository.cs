using MeetDown.Events.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetDown.Events.Core.DataAccess.Repositories
{
    public interface IMeetRepository
    {
        IEnumerable<Group> GetGroups();

        PagedList<Group> GetGroups(int pageNumber, int pageSize, IEnumerable<ISortCriterion<Group>> sortCriteria);

        bool GroupExists(int groupId);

        Group GetGroup(int id, bool includeMeets = false);

        void AddGroup(Group group);

        void RemoveGroup(Group group);

        PagedList<Meet> GetMeets(int groupId, int pageNumber, int pageSize, IEnumerable<ISortCriterion<Meet>> sortCriteria);

        Meet GetMeet(int groupId, int id);

        void AddMeetToGroup(int groupId, Meet meet);

        void RemoveMeet(Meet meet);

        bool Save();
    }
}
