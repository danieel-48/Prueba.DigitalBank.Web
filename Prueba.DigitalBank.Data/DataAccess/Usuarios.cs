using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.DigitalBank.Data.DataAccess
{
    public class Usuarios 
    {
        private readonly Conexion _conexion;
        public Usuarios(Conexion conexion)
        {
            _conexion = conexion;
        }
        /// <summary>
        /// Ejecucion del SP que realiza el CRUD
        /// </summary>
        /// <param name="accion"></param>
        /// <param name="id"></param>
        /// <param name="nombre"></param>
        /// <param name="fecha"></param>
        /// <param name="sexo"></param>
        /// <returns></returns>
        public DataTable EjecutarSP(string accion, int? id = null, string? nombre = null, DateTime? fecha = null, string? sexo = null)
        {
            var dt = new DataTable();

            try
            {
                using var conn = _conexion.GetConnection();
                using var cmd = new SqlCommand("sp_Usuarios_CRUD", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Accion", accion);
                cmd.Parameters.AddWithValue("@Id", id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombre", nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fecha ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", sexo ?? (object)DBNull.Value);

                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (SqlException sqlEx)
            {

                Console.WriteLine($"Error de SQL: {sqlEx.Message}");
                if (sqlEx.InnerException != null)
                    Console.WriteLine($"InnerException: {sqlEx.InnerException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
            return dt; 
        }
    }
}
