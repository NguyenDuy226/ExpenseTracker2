using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.API.Services
{
    public class createJWT
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly IConfiguration _iconfiguration;
        public createJWT (UserManager<AppUser> userManager, IConfiguration _configuration)
        {
            this._userManager = userManager;
            this._iconfiguration = _configuration;
        }
        public async Task<string> create(AppUser user)
        {
            //key, cred, claim
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfiguration["JWT:Key"]?? string.Empty));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Name, user.Name),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            };
            var expires = _iconfiguration.GetValue<int>("JWT:ExpiryMinute");
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: cred,
                expires: DateTime.UtcNow.AddMinutes(expires),
                issuer: _iconfiguration["JWT:Issuer"],
                audience: _iconfiguration["JWT:Audience"]
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}