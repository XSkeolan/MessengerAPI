using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MessengerAPI.Controllers
{
    [Route("public/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ISignUpService _signUpService;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public RegisterController(ISignUpService signUp, IOptions<JWTOptions> options)
        {
            _signUpService = signUp;
            _jwtKey = options.Value.Key;
            _jwtIssuer = options.Value.Issuer;
            _jwtAudience = options.Value.Audience;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(SignUpRequestUser inputUser)
        {
            if (!Regex.IsMatch(inputUser.Phonenumber, @"\d{11}") || inputUser.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.PHONENUMBER_INCORRECT);
            }

            if (inputUser.Name.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (inputUser.Surname.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (inputUser.Password.Length > 32 || inputUser.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }
            if(inputUser.Nickname.Length > 20)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            User user = new User
            {
                Phonenumber = inputUser.Phonenumber,
                Name = inputUser.Name,
                Surname = inputUser.Surname,
                Nickname = inputUser.Nickname
            };

            try
            {
                SignInResponseUserInfo signIn = await _signUpService.SignUp(user, Request.Headers.UserAgent, inputUser.Password);
                signIn.Token = GenerateJwtToken(user.Phonenumber);
                return Ok(signIn);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJwtToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("phonenumber", userName) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet(nameof(GetResult))]
        public IActionResult GetResult()
        {
            return Ok("API Validated");
        }
    }
}
