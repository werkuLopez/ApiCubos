using ApiCubos.Helpers;
using ApiCubos.Models;
using ApiCubos.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private CubosRepository repo;
        private HelperActionServicesOAuth helper;

        public AuthController(CubosRepository repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> LogIn(string username, string password)
        {
            Usuario usuario =
                await this.repo.LogIn(username, password);

            if (usuario != null)
            {
                SymmetricSecurityKey secretKey = this.helper.GetKeyToken();

                SigningCredentials credentials =
                    new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                string jsonUsuario =
                    JsonConvert.SerializeObject(usuario);

                Claim[] usuariojson = new[]
                {
                    new Claim("UsuarioData", jsonUsuario)
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                            claims: usuariojson,
                            issuer: this.helper.Issuer,
                            audience: this.helper.Audience,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            notBefore: DateTime.UtcNow,
                            signingCredentials: credentials
                        );

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { response = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<ActionResult<Usuario>> Register(Usuario usuario)
        //{

        //}
    }
}
