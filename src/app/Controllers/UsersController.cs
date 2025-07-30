using Microsoft.AspNetCore.Mvc;
using app.Models;
using app.Service;
using app.Interface;
using app.DTOs.Auth;

namespace app.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private MsgService _msgService = new MsgService();

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPut("SwitchStatus")]
        public async Task<ActionResult> SwitchStatus(string email)
        {
            return new JsonResult(new
            {
                EC = 0,
                EM = await _userRepo.SwitchStatus(email)
            });
        }

        [HttpGet("getAllUser")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsers();
            return _msgService.MsgReturn(0, "success", users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _userRepo.getUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(int userId, UserProfileForm user)
        //{
        //    await _userRepo.updateUser(userId, user);
        //    return NoContent();
        //}

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            await _userRepo.addNewUser(user);
            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }
    }
}
