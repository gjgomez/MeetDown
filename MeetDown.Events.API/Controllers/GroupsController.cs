using AutoMapper;
using MeetDown.Events.Core.DataAccess.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MeetDown.Events.API.Controllers
{
    // TODO: Consider using async/await

    [Route("/api/groups")]
    public class GroupsController : Controller
    {
        private IMeetRepository _meetRepository;

        public GroupsController(IMeetRepository meetRepository)
        {
            _meetRepository = meetRepository;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var groupEntities = _meetRepository.GetGroups();
            var groups = Mapper.Map<IEnumerable<ViewModels.Groups.ReadViewModel>>(groupEntities);

            return Ok(groups);
        }

        [HttpGet("{id}", Name = "GetGroup")]
        public IActionResult Get(int id, bool includeMeets = false)
        {
            var groupEntity = _meetRepository.GetGroup(id, includeMeets);

            if (groupEntity == null)
            {
                return NotFound();
            }

            var group = Mapper.Map<ViewModels.Groups.ReadViewModel>(groupEntity);
                
            return Ok(group);
        }

        [HttpPost()]
        public IActionResult Create([FromBody] ViewModels.Groups.CreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = Mapper.Map<Core.Entities.Group>(viewModel);

            _meetRepository.AddGroup(group);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return CreatedAtRoute("GetGroup", new { id = group.Id }, group);
        }

        [HttpPut("{id}")]
        public IActionResult FullUpdate(int id, [FromBody] ViewModels.Groups.UpdateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = _meetRepository.GetGroup(id);

            if (group == null)
            {
                return NotFound();
            }

            Mapper.Map(viewModel, group);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<ViewModels.Groups.UpdateViewModel> viewModel)
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

            var groupEntity = _meetRepository.GetGroup(id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            var groupToPatch = Mapper.Map<ViewModels.Groups.UpdateViewModel>(groupEntity);

            viewModel.ApplyTo(groupToPatch, ModelState);

            // ensure the meetToPatch model is valid
            TryValidateModel(groupToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(groupToPatch, groupEntity);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var group = _meetRepository.GetGroup(id);

            if (group == null)
            {
                return NotFound();
            }

            _meetRepository.RemoveGroup(group);

            if (!_meetRepository.Save())
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }

            return NoContent();
        }
    }
}
