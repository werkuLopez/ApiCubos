using ApiCubos.Models;
using ApiCubos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private CubosRepository repo;

        public CubosController(CubosRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> Cubos()
        {
            List<Cubo> cuubs =
                await this.repo.GetAllCubosAsync();

            return Ok(cuubs);
        }

        [HttpGet]
        [Route("{marca}")]
        public async Task<ActionResult<List<Cubo>>> CubosMarca(string marca)
        {
            List<Cubo> cuubs =
                    await this.repo.GetAllCubosByMarcaAsync(marca);

            return Ok(cuubs);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CompraCubo>>> Pedidos()
        {
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UsuarioData");
            string jsonUsuario = claim.Value;

            Usuario usuario =
                JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            List<CompraCubo> pedidos =
                await this.repo.GetPedidosUsuario(usuario.IdUsuario);

            return Ok(pedidos);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> Perfil()
        {
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UsuarioData");
            string jsonUsuario = claim.Value;

            Usuario usuario =
                JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            Usuario user =
                await this.repo.PerfilusuarioAsync(usuario.IdUsuario);

            return Ok(user);
        }

    }
}
