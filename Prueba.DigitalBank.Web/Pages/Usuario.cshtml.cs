using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Prueba.DigitalBank.Api.Models;

namespace Prueba.DigitalBank.Web.Pages
{
    public class UsuarioModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UsuariosDTO NuevoUsuario { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// Consulta de Usuarios
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("UsuariosApi");
                    var response = await client.GetAsync($"{id.Value}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var usuario = JsonSerializer.Deserialize<UsuariosDTO>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (usuario != null)
                            NuevoUsuario = usuario;
                        else
                            Mensaje = "No se encontró el usuario.";
                    }
                    else
                    {
                        Mensaje = $"Error al obtener usuario: {response.StatusCode}";
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Mensaje = $"Error de conexión al obtener usuario: {httpEx.Message}";
                }
                catch (Exception ex)
                {
                    Mensaje = $"Ocurrió un error al obtener usuario: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Metodo para editar o crear segun corresponda
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("UsuariosApi");
                var json = JsonSerializer.Serialize(NuevoUsuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                // Si el ID es mayor a cero es un Update
                if (NuevoUsuario.Id > 0)
                    response = await client.PutAsync($"{NuevoUsuario.Id}", content);
                else
                    response = await client.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    Mensaje = NuevoUsuario.Id > 0
                        ? "Usuario modificado correctamente."
                        : "Usuario registrado correctamente.";

                    return RedirectToPage("UsuarioConsulta");
                }
                else
                {
                    Mensaje = $"Error al guardar usuario: {response.StatusCode}";
                    return Page();
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Error de red o conexión
                Mensaje = $"Error de conexión al guardar usuario: {httpEx.Message}";
                return Page();
            }
            catch (Exception ex)
            {
                // Cualquier otro error inesperado
                Mensaje = $"Ocurrió un error al guardar usuario: {ex.Message}";
                return Page();
            }
        }
    }
}
