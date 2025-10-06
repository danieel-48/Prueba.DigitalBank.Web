using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.DigitalBank.Data.DataAccess
{
    public class Conexion
    {
        private readonly string _connectionString;
        public Conexion(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public SqlConnection GetConnection() => new SqlConnection(_connectionString);
    }
}
