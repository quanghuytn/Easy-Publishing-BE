using EP.Application.Commands.Users;
using EP.Application.Queries.User;
using EP.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("SwitchStatus")]
        public async Task<ActionResult> SwitchStatus(string email)
        {
            var command = new SwitchUserStatusCommand(email);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("getAllUser")]
        public async Task<ActionResult> GetAllUsers()
        {
            var query = new GetAllUserQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        // GET: api/Users/5
        //[HttpGet("{id}")]
        //public ActionResult<User> GetUser(int id)
        //{
        //    var user = _userRepo.getUserById(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var command = new AddNewUserCommand(user);
            var response = await _mediator.Send(command);

            return CreatedAtAction("GetUser", new { id = response.UserId }, response);
        }
    }
}
