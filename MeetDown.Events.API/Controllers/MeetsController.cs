using AutoMapper;
using MeetDown.Events.Core.DataAccess.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MeetDown.Events.API.Controllers
{
    [Route("api/groups")]
    public class MeetsController : Controller
    {
        private IMeetRepository _meetRepository;

        public MeetsController(IMeetRepository meetRepository)
        {
            _meetRepository = meetRepository;
        }

        [HttpGet("{groupId}/meets")]
        public IActionResult Get(int groupId)
        {
            var groupEntity = _meetRepository.GetGroup(groupId, true);

            if (groupEntity == null)
            {
                return NotFound();
            }

            var meets = Mapper.Map<IEnumerable<ViewModels.Meets.ReadViewModel>>(groupEntity.Meets);

            return Ok(meets);
        }

        [HttpGet("{groupId}/meets/{id}", Name = "GetMeet")]
        public IActionResult Get(int groupId, int id)
        {
            var meetEntity = _meetRepository.GetMeet(groupId, id);

            if (meetEntity == null)
            {
                return NotFound();
            }

            var meet = Mapper.Map<ViewModels.Meets.ReadViewModel>(meetEntity);

            return Ok(meet);
        }

        [HttpPost("{groupId}/meets")]
        public IActionResult Create(int groupId, [FromBody] ViewModels.Meets.CreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_meetRepository.GroupExists(groupId))
            {
                return NotFound();
            }

            var meet = Mapper.Map<Core.Entities.Meet>(viewModel);

            _meetRepository.AddMeetToGroup(groupId, meet);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            var createdMeet = Mapper.Map<ViewModels.Meets.ReadViewModel>(meet);

            return CreatedAtRoute("GetMeet", new { groupId = groupId, id = meet.Id }, createdMeet);
        }

        [HttpPut("{groupId}/meets/{id}")]
        public IActionResult FullUpdate(int groupId, int id, [FromBody] ViewModels.Meets.UpdateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetEntity = _meetRepository.GetMeet(groupId, id);
            
            if (meetEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(viewModel, meetEntity);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }

        [HttpPatch("{groupId}/meets/{id}")]
        public IActionResult PartialUpdate(int groupId, int id, [FromBody] JsonPatchDocument<ViewModels.Meets.UpdateViewModel> viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            // ensure the JSON patch document is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetEntity = _meetRepository.GetMeet(groupId, id);

            if (meetEntity == null)
            {
                return NotFound();
            }

            var meetToPatch = Mapper.Map<ViewModels.Meets.UpdateViewModel>(meetEntity);

            viewModel.ApplyTo(meetToPatch, ModelState);

            // ensure the meetToPatch model is valid
            TryValidateModel(meetToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(meetToPatch, meetEntity);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }

        [HttpDelete("{groupId}/meets/{id}")]
        public IActionResult Delete(int groupId, int id)
        {
            var meet = _meetRepository.GetMeet(groupId, id);

            if (meet == null)
            {
                return NotFound();
            }

            _meetRepository.RemoveMeet(meet);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }
    }
}
