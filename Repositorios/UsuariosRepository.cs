using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;  // Usamos esta librería
using EspacioTp5;  // Asegúrate de que esta es la clase del modelo `Usuarios`

namespace rapositoriosTP5
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";

        public void CrearUsuario(Usuarios usuario)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "INSERT INTO Usuarios (Nombre, Usuario, Contraseña, Rol) VALUES (@Nombre, @Usuario, @Contraseña, @Rol)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                    command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Usuarios> ObtenerUsuarios()
        {
            var usuarios = new List<Usuarios>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new Usuarios(
                                Convert.ToInt32(reader["id_usuario"]),
                                reader["Nombre"].ToString(),
                                reader["Usuario"].ToString(),
                                reader["Contraseña"].ToString(),
                                reader["Rol"].ToString()
                            );
                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return usuarios;
        }

        public Usuarios BuscarUsuarioPorId(int id)
        {
            Usuarios usuario = null;

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios WHERE id_usuario = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuarios(
                                Convert.ToInt32(reader["id_usuario"]),
                                reader["Nombre"].ToString(),
                                reader["Usuario"].ToString(),
                                reader["Contraseña"].ToString(),
                                reader["Rol"].ToString()
                            );
                        }
                    }
                }
            }

            return usuario;
        }

        public void ModificarUsuario(Usuarios usuario)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "UPDATE Usuarios SET Nombre = @Nombre, Usuario = @Usuario, Rol = @Rol WHERE id_usuario = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", usuario.IdUsuario);
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ModificarPassword(int id, string nuevaPassword)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "UPDATE Usuarios SET Contraseña = @Contraseña WHERE id_usuario = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Contraseña", nuevaPassword);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarUsuario(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "DELETE FROM Usuarios WHERE id_usuario = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CambiarRol(int id, string nuevoRol)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "UPDATE Usuarios SET Rol = @Rol WHERE id_usuario = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Rol", nuevoRol);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
