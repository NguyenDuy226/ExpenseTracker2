using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class authController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly createJWT _createJWT;
        public authController (UserManager<AppUser> userManager,  RoleManager<IdentityRole> roleManager, createJWT _createJWT)
        {   
            _userManager = userManager;
            _roleManager = roleManager;
            this._createJWT = _createJWT;
        }
        [HttpPost("register")]
        // POST /auth/register
        public async Task<IActionResult> postRegister (registerDTO dto)
        {
            var user = new AppUser()
            {
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.Email   
            };
            var checkEmail = await _userManager.FindByEmailAsync(user.Email);
            if(checkEmail != null) return Conflict();
            //hash password
            var create = await _userManager.CreateAsync(user, dto.Password);
            if(!create.Succeeded) return BadRequest(create.Errors);
            //add role
            if(!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            var addRole = await _userManager.AddToRoleAsync(user, "User");
            if(!addRole.Succeeded) return BadRequest(addRole.Errors);
            return Ok();
        }
        [HttpPost("login")]
        //POST /auth/login 
        public async Task<IActionResult> postLogin (loginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null) return NotFound();
            if(! await _userManager.CheckPasswordAsync(user, dto.Password)) return Unauthorized();
            var token = await _createJWT.create(user);
            return Ok (
                token
            );
        }

        [Authorize]
        [HttpGet("check")]
        public IActionResult Checked() => Ok("checked");
    }
}