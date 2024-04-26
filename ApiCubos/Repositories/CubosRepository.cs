using ApiCubos.Data;
using ApiCubos.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace ApiCubos.Repositories
{
    public class CubosRepository
    {
        private CubosContext context;

        public CubosRepository(CubosContext context)
        {

            this.context = context;

        }

        public async Task<int> GetMaxIdUsuario()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Usuarios.MaxAsync(x => x.IdUsuario) + 1;
            }
        }

        public async Task<int> GetMaxIdPedido()
        {
            if (this.context.CompraCubos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.CompraCubos.MaxAsync(x => x.IdPedido) + 1;
            }
        }

        public async Task<List<Cubo>> GetAllCubosAsync()
        {
            List<Cubo> cubos = await this.context.Cubos.ToListAsync();
            List<Cubo> news = new List<Cubo>();

            foreach (var item in cubos)
            {
                string img = "https://storageexamencubowerku.blob.core.windows.net/cubosexam/ " + item.Imagen;
                item.Imagen = img;

                news.Add(item);
            }

            return news;
        }

        public async Task<List<Cubo>> GetAllCubosByMarcaAsync(string marca)
        {
            List<Cubo> cubos = await this.context.Cubos.Where(x => x.Marca == marca).ToListAsync();
            List<Cubo> news = new List<Cubo>();

            foreach (var item in cubos)
            {
                string img = "https://storageexamencubowerku.blob.core.windows.net/cubosexam/ " + item.Imagen;
                item.Imagen = img;

                news.Add(item);
            }

            return news;
        }

        public async Task<Usuario> CreateUsuario(Usuario usuario)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuario();
            user.Nombre = usuario.Nombre;
            user.Email = usuario.Email;
            user.Pass = usuario.Pass;
            user.Imagen = usuario.Imagen;

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();

            return user;

        }

        // compras usuario token
        public async Task<List<CompraCubo>> GetPedidosUsuario(int idusuario)
        {
            return await this.context.CompraCubos.Where(x => x.Idusuario == idusuario).ToListAsync();
        }


        // perfil token
        public async Task<Usuario> PerfilusuarioAsync(int idusuario)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idusuario);
        }

        // realizar pedido
        public async Task<CompraCubo> RealizarPedidoAsync(CompraCubo compraCubo)
        {
            CompraCubo compra = new CompraCubo();
            compra.IdPedido = await this.GetMaxIdPedido();
            compra.IdCubo = compraCubo.IdCubo;
            compra.Idusuario = compraCubo.Idusuario;
            compra.Fecha = DateTime.Now;

            this.context.CompraCubos.Add(compra);
            await this.context.SaveChangesAsync();

            return compra;
        }


        // auth

        public async Task<Usuario> LogIn(string username, string pass)
        {
            Usuario user = await this.context.Usuarios.Where(x => x.Email == username && x.Pass == pass).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }
    }
}
