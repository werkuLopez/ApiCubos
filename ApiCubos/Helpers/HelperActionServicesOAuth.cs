using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace ApiCubos.Helpers
{
    public class HelperActionServicesOAuth
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        //private SecretClient client;
        //private string storageKey;

        //private BlobServiceClient clientBlobs;


        public HelperActionServicesOAuth(IConfiguration config)
        {

            var keyVaultUri = config.GetValue<string>("KeyVault:VaultUri");

            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

            this.Issuer = GetSecretValue(secretClient, "Issuer");
            this.Audience = GetSecretValue(secretClient, "Audience");
            this.SecretKey = GetSecretValue(secretClient, "SecretKey");
            this.SecretKey = GetSecretValue(secretClient, "SecretKey");
            //this.storageKey = GetSecretValue(secretClient, "StorageKey");

        }

        private string GetSecretValue(SecretClient secretClient, string secretName)

        {
            try
            {
                KeyVaultSecret secret = secretClient.GetSecret(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"No se pudo obtener el secreto '{secretName}' del Key Vault.", ex);
            }

        }

        // necesitamos un método para generar el token que se basa en el secretKey

        public SymmetricSecurityKey GetKeyToken()
        {
            // byte[]
            byte[] data =
                Encoding.UTF8.GetBytes(this.SecretKey);
            // devolvemos la key generada mediante byte[]
            return new SymmetricSecurityKey(data);
        }

        // hemos creado la clase para quitar código en el program
        // Método para validar el token
        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options =
            new Action<JwtBearerOptions>(options =>
            {
                // indicamos que deseamos validar el token
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken()
                };
            });

            return options;
        }

        // Método para indicar el esquema de la validacion

        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                });

            return options;
        }
    }
}
