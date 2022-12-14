using CoreWebApiRepository.IRepository;
using CoreWebApiRepository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CoreWebApiRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        IUserRepository _oUserRepository=null;

        public UserController(IConfiguration config, IUserRepository ouserRepository)
        {
            _config = config;
            this._oUserRepository = ouserRepository;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] User model)
        {
            try
            {
                model.Password = Passhashing(model.Password);
                model = await _oUserRepository.Save(model);
                return Ok(model);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,ex.Message);
            }
        }
        [HttpGet]
        [Route("Signin/{username}/{password}")]
        public async Task<IActionResult> Signin(string username,string password)
        {
            try
            {
                User model = new User();
                model.UserName = username;
                model.Password = Passhashing(password);

                var user = await _oUserRepository.GetByUsernamePassword(model);
                if(user.UserId==0)return StatusCode((int)HttpStatusCode.NotFound,"Invalid User");
                user.Token = GenerateToken(model);
               
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string GenerateToken(User model)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256);

            var token=new JwtSecurityToken(_config["Jwt:Issuer"],_config["Jwt:Issuer"]
                ,null
                ,expires:DateTime.Now.AddMinutes(120)
                ,signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string Passhashing(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}
