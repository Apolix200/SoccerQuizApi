using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SoccerQuizApi.Helper;
using SoccerQuizApi.Models;
using SoccerQuizApi.Services;

namespace SoccerQuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _usersService;
        private readonly AdminHelper _adminHelper;

        public UserController(UserService usersService, AdminHelper adminHelper)
        {
            _usersService = usersService;
            _adminHelper = adminHelper;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get(string adminId)
        {
            var users = await _usersService.GetAsync();

            if(await _adminHelper.NotAdmin(adminId))
            {
                return new List<User>();
            }

            return users.Where(w => w.IsAdmin == false);          
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<User>> Login(User newUser)
        {
            var users = await _usersService.GetAsync();

            var user = users.FirstOrDefault(u => u.UserName == newUser.UserName);

            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Password != newUser.Password)
            {
                return StatusCode(403);
            }

            user.Password = "";

            return user;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Register(User newUser)
        {
            var users = await _usersService.GetAsync();

            if (users.Any(u => u.UserName == newUser.UserName))
            {
                return UnprocessableEntity();
            }
            if (newUser.Password.Length < 8)
            {
                return StatusCode(420);
            }

            newUser.IsAdmin = false;

            await _usersService.CreateAsync(newUser);

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id, string adminId)
        {
            if (await _adminHelper.NotAdmin(adminId))
            {
                return Unauthorized();
            }

            var user = await _usersService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            await _usersService.RemoveAsync(id);

            return NoContent();
        }
    }
}
