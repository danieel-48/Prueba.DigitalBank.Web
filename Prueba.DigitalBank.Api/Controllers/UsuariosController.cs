using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prueba.DigitalBank.Api.Models;
using Prueba.DigitalBank.Data.DataAccess;
using System.Data;


namespace Prueba.DigitalBank.Api.Controllers
{ [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly Usuarios _user;
        public UsuariosController(Usuarios repo)
        {
            _user = repo;
        }

        /// <summary>
        /// Consulta Usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public IActionResult GetUsuarios()
        {
            try
            {
                var dt = _user.EjecutarSP("CONSULTAR");
                var lista = new List<UsuariosDTO>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new UsuariosDTO
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nombre = row["Nombre"].ToString(),
                        FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                        Sexo = row["Sexo"].ToString()
                    });
                }
                return Ok(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return StatusCode(500, $"Error al consultar Usuarios: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUsuarioById(int id)
        {
            try
            {
                var dt = _user.EjecutarSP("CONSULTAR");
                var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["Id"]) == id);

                if (row == null)
                    return NotFound();

                var usuario = new UsuariosDTO
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["Nombre"].ToString(),
                    FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                    Sexo = row["Sexo"].ToString()
                };

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return StatusCode(500, $"Error al consulta Usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Registrar Usuarios
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public IActionResult AgregarUsuario([FromBody] UsuariosDTO u)
        {
            if (u == null || string.IsNullOrWhiteSpace(u.Nombre) || string.IsNullOrWhiteSpace(u.Sexo))
                return BadRequest("Datos incompletos del usuario.");

            try
            {
                _user.EjecutarSP("AGREGAR", null, u.Nombre, u.FechaNacimiento, u.Sexo);
                return Ok(new { mensaje = "Usuario agregado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return StatusCode(500, $"Error al agregar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Modificar Usuarios
        /// </summary>
        /// <param name="id"></param>
        /// <param name="u"></param>
        /// <returns></returns>

        [HttpPut("{id}")]
        public IActionResult ModificarUsuario(int id, [FromBody] UsuariosDTO u)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            if (u == null || string.IsNullOrWhiteSpace(u.Nombre) || string.IsNullOrWhiteSpace(u.Sexo))
                return BadRequest("Datos incompletos del usuario.");

            try
            {
                _user.EjecutarSP("MODIFICAR", id, u.Nombre, u.FechaNacimiento, u.Sexo);
                return Ok(new { mensaje = "Usuario modificado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return StatusCode(500, $"Error al modificar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Eliminar Usuarios
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            try
            {
                _user.EjecutarSP("ELIMINAR", id);
                return Ok(new { mensaje = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return StatusCode(500, $"Error al eliminar usuario: {ex.Message}");
            }
        }
    }
}
