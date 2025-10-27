using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;

namespace AgricolaDH_GApp.Services.Admin
{
    public class UsuarioService
    {
        private readonly AppDbContext context;
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly ILogger<UsuarioService> _logger;


        public UsuarioService(AppDbContext _ctx, IConfiguration configuration, ILogger<UsuarioService> logger)
        {
            context = _ctx;
            // Retrieve the AAD settings from the configuration
            _tenantId = configuration["AzureAD:TenantId"];
            _clientId = configuration["AzureAD:ClientId"];
            _clientSecret = configuration["AzureAD:ClientSecret"];
            _logger = logger;
            _logger.LogInformation($"TenantId: {_tenantId}, ClientId: {_clientId}, ClientSecret: {(_clientSecret != null ? "OK" : "NULL")}");
        }

        public Usuario GetUsuarioById(int id)
        {
            return context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
        }


        public Usuario UsuarioLogin(string username, string password)
        {
            Usuario usuario;

            try
            {
                usuario = context.Usuarios.FromSqlRaw("EXEC SP_SelectUsuarioLogin @Username, @Password",
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)).ToList().FirstOrDefault();

            }
            catch
            {
                usuario = new Usuario();
            }

            return usuario;
        }

        public List<Usuario> SelectUsuarios()
        {
            List<Usuario> usuarioList;

            try
            {
                usuarioList = context.Usuarios.ToList();
            }
            catch
            {
                usuarioList = new List<Usuario>();
            }

            return usuarioList;
        }

        public List<UsuarioDropdown> SelectUsuariosByIdRol(int IdRol)
        {

            List<UsuarioDropdown> usuarioDropdown;

            try
            {
                usuarioDropdown = context.UsuariosDropdown.FromSqlRaw("exec SP_SelectIngenierosDropdown @IdRol",
                    new SqlParameter("@IdRol", IdRol)).ToList();
            }
            catch
            {
                usuarioDropdown = new List<UsuarioDropdown>();
            }

            return usuarioDropdown;
        }

        public Usuario SelectUsuario(int IdUsuario)
        {
            Usuario usuario;

            try
            {
                usuario = context.Usuarios.Find(IdUsuario);
            }
            catch
            {
                usuario = new Usuario();
            }

            return usuario;
        }

        public int InsertUsuario(Usuario usuario)
        {
            int res = 0;

            try
            {
                context.Usuarios.Add(usuario);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateUsuario(Usuario usuario)
        {
            int res = 0;

            try
            {
                context.Usuarios.Update(usuario);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteUsuario(int IdUsuario)
        {
            int res = 0;

            try
            {
                Usuario usuario = context.Usuarios.Find(IdUsuario);

                context.Usuarios.Remove(usuario);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }

        public async Task<int> AddToActiveDirectory(Usuario usuario)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var token = await GetAccessTokenAsync(_tenantId, _clientId, _clientSecret);
                    _logger.LogInformation($"Access Token: {token}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var invitationData = new
                    {
                        invitedUserEmailAddress = usuario.Correo,
                        inviteRedirectUrl = "https://example.com", // URL to redirect user after accepting the invitation
                        sendInvitationMessage = true,
                        invitedUserDisplayName = $"{usuario.Nombre} {usuario.ApellidoPaterno}",
                        invitedUserMessageInfo = new
                        {
                            subject = "Has sido invitado a la aplicacion web DHG & Agricola.",
                            customizedMessageBody = "Un usuario y contraseña se te han asignado. Por favor acepta la invitacion e ingresa con tus credenciales."
                        }
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(invitationData), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://graph.microsoft.com/v1.0/invitations", content);
                    _logger.LogInformation($"Response Code: {response.StatusCode} | Message: {response.Content}");
                    if (!response.IsSuccessStatusCode)
                        return -1;
                }
            }
            catch(Exception e)
            {
                _logger.LogWarning(e.Message);
                return -1;
            }
            return 0;
        }

        public async Task<int> UpdateActiveDirectory(Usuario old, Usuario newUser)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var token = await GetAccessTokenAsync(_tenantId, _clientId, _clientSecret);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    //Get user from ActiveDirectory
                    string displayName = $"{old.Nombre} {old.ApellidoPaterno}";
                    string userId = await GetUserIdFromActiveDirectory(displayName);
                    if (userId == null)
                        return -1;
                    var userData = new
                    {
                        displayName = $"{newUser.Nombre} {newUser.ApellidoPaterno}",
                    };
                    // Prepare PATCH request
                    var content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, "application/json");
                    var patchResponse = await client.PatchAsync($"https://graph.microsoft.com/v1.0/users/{userId}", content);
                    if (!patchResponse.IsSuccessStatusCode)
                        return -1;
                }
            }
            catch 
            {
                return -1;
            }
            return 0;
        }

        public async Task<int> DropActiveDirectory(Usuario usuario)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var token = await GetAccessTokenAsync(_tenantId, _clientId, _clientSecret);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string displayName = $"{usuario.Nombre} {usuario.ApellidoPaterno}";
                    string userId = await GetUserIdFromActiveDirectory(displayName);
                    if (userId == null)
                        return -1;
                    var response = await client.DeleteAsync($"https://graph.microsoft.com/v1.0/users/{userId}");
                    if (!response.IsSuccessStatusCode)
                        return -1;
                }
            }
            catch(Exception e)
            {
               _logger.LogWarning(e.Message);
                return -1;
            }
            return 0;
        }

        private async Task<string> GetAccessTokenAsync(string tenantId, string clientId, string clientSecret)
        {
            _logger.LogInformation($"TenantId: {tenantId}, ClientId: {clientId}, ClientSecret: {(clientSecret != null ? "OK" : "NULL")}");
            try
            {
                var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                var body = new Dictionary<string, string>
                {
                    {"client_id", clientId},
                    {"scope", "https://graph.microsoft.com/.default"},
                    {"client_secret", clientSecret},
                    {"grant_type", "client_credentials"}
                };

                using var client = new HttpClient();
                var res = await client.PostAsync(url, new FormUrlEncodedContent(body));
                var json = await res.Content.ReadAsStringAsync();
                var tokenObj = JsonConvert.DeserializeObject<JObject>(json);
;               _logger.LogInformation($"Access Token: {tokenObj["access_token"]} | Message: {res.Content}");
                return tokenObj["access_token"]?.ToString();
            }
            catch(Exception e)
            {
                _logger.LogWarning(e.Message);
                return "";
            }
        }

        private async Task<string> GetUserIdFromActiveDirectory(string displayName)
        {
            string userId;
            try
            {
                using (var client = new HttpClient())
                {
                    var token = await GetAccessTokenAsync(_tenantId, _clientId, _clientSecret);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var userResponse = await client.GetAsync($"https://graph.microsoft.com/v1.0/users?$filter=startswith(displayName,'{displayName}')");
                    if (!userResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await userResponse.Content.ReadAsStringAsync();
                        return null;
                    }
                    var jsonUser = await userResponse.Content.ReadAsStringAsync();
                    var user = JObject.Parse(jsonUser)["value"] as JArray;
                    if (user == null || user.Count == 0)
                    {
                        return null;
                    }
                    userId = user[0]["id"]?.ToString();
                }
            }
            catch
            {
                return null;

            }
            return userId;
        }
    }
}
