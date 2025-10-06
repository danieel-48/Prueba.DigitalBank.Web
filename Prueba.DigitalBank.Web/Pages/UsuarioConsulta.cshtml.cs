using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Prueba.DigitalBank.Api.Models;

namespace Prueba.DigitalBank.Web.Pages
{
    public class UsuarioConsultaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioConsultaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<UsuariosDTO> Usuarios { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await CargarUsuarios();
        }

        /// <summary>
        /// Metodo para consumir API para eliminar Usuarios
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("UsuariosApi");
                var response = await client.DeleteAsync($"{id}");

                if (response.IsSuccessStatusCode)
                {
                    Mensaje = "Usuario eliminado correctamente.";
                }
                else
                {
                    Mensaje = $"Error al eliminar usuario: {response.StatusCode}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine(httpEx.InnerException);
                Mensaje = $"Error de conexión al eliminar usuario: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Mensaje = $"Ocurrió un error al eliminar usuario: {ex.Message}";
            }

            await CargarUsuarios();
            return Page();
        }

        /// <summary>
        /// Metodo para consultar los usuarios 
        /// </summary>
        /// <returns></returns>
        private async Task CargarUsuarios()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("UsuariosApi");
                var response = await client.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Usuarios = JsonSerializer.Deserialize<List<UsuariosDTO>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UsuariosDTO>();
                }
                else
                {
                    Usuarios = new List<UsuariosDTO>();
                    Mensaje = $"Error al cargar usuarios: {response.StatusCode}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                Usuarios = new List<UsuariosDTO>();
                Mensaje = $"Error de conexión al cargar usuarios: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                Usuarios = new List<UsuariosDTO>();
                Mensaje = $"Ocurrió un error al cargar usuarios: {ex.Message}";
            }
        }
    }
}
