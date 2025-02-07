using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using EspacioTp5;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace rapositoriosTP5
{
    public class RepositorioUsuariosSqlite : IUsuariosRepository
    {
        private readonly string cadenaConexion;
        private readonly ILogger<RepositorioUsuariosSqlite> _logger;

        public RepositorioUsuariosSqlite(string cadenaConexion, ILogger<RepositorioUsuariosSqlite> logger)
        {
            this.cadenaConexion = cadenaConexion;
            _logger = logger;
        }

        public Usuarios ObtenerUsuario(string usuario, string contraseña)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                throw new ArgumentException("Usuario y contraseña no pueden estar vacíos.");
            }

            try
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
                throw new KeyNotFoundException("Usuario no encontrado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario");
                throw;
            }
        }

        public List<Usuarios> ListarUsuarios()
        {
            var usuarios = new List<Usuarios>();

            try
            {
                using (var conexion = new SqliteConnection(cadenaConexion))
                {
                    conexion.Open();
                    var comando = conexion.CreateCommand();
                    comando.CommandText = "SELECT id_usuario, Nombre, Usuario, Contraseña, Rol FROM Usuarios";

                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var usuario = new Usuarios(
                                lector.GetInt32(lector.GetOrdinal("id_usuario")),
                                lector.GetString(lector.GetOrdinal("Nombre")),
                                lector.GetString(lector.GetOrdinal("Usuario")),
                                lector.GetString(lector.GetOrdinal("Contraseña")),
                                lector.GetString(lector.GetOrdinal("Rol"))
                            );
                            usuarios.Add(usuario);
                        }
                    }
                }
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los usuarios");
                throw;
            }
        }

        public Usuarios ObtenerUsuarioPorId(int id)
        {
            try
            {
                using (var conexion = new SqliteConnection(cadenaConexion))
                {
                    conexion.Open();
                    var comando = conexion.CreateCommand();
                    comando.CommandText = @"
                        SELECT id_usuario, Nombre, Usuario, Contraseña, Rol
                        FROM Usuarios
                        WHERE id_usuario = @id";

                    comando.Parameters.AddWithValue("@id", id);

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
                throw new KeyNotFoundException("Usuario no encontrado con ID " + id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario por ID");
                throw;
            }
        }

        public int? ObtenerIdClientePorUsuario(int idUsuario)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "SELECT idCliente FROM Usuarios WHERE id_usuario = @idUsuario";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : (int?)null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ID de cliente por usuario");
                throw;
            }
        }
    }
}
