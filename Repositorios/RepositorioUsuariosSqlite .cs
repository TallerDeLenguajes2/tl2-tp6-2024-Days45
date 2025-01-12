using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using EspacioTp5;

namespace rapositoriosTP5
{
    public class RepositorioUsuariosSqlite : IUsuariosRepository
    {
        private string cadenaConexion;

        public RepositorioUsuariosSqlite(IConfiguration configuracion)
        {
            cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
        }

        public Usuarios ObtenerUsuario(string usuario, string contraseña)
        {
            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                var comando = conexion.CreateCommand();
                comando.CommandText = @"
                    SELECT id_usuario, Nombre, Usuario, Contraseña, Rol
                    FROM Usuarios
                    WHERE Usuario = @usuario AND Contraseña = @contraseña";

                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@contraseña", contraseña);

                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Usuarios(
                            lector.GetInt32(lector.GetOrdinal("id_usuario")),
                            lector.GetString(lector.GetOrdinal("Nombre")),
                            lector.GetString(lector.GetOrdinal("Usuario")),
                            lector.GetString(lector.GetOrdinal("Contraseña")),
                            lector.GetString(lector.GetOrdinal("Rol"))
                        );
                    }
                }
            }
            // Devuelve null si no se encuentra el usuario
            return null;
        }
    }
}
