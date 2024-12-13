using System.Linq;
using System.Collections.Generic;
namespace rapositoriosTP5;
using EspacioTp5;
public class RepositorioUsuariosEnMemoria : IUsuariosRepository
{
    private readonly List<Usuarios> _usuarios;

    public RepositorioUsuariosEnMemoria()
    {
        // Lista de usuarios predefinidos para simular una base de datos
        _usuarios = new List<Usuarios>
        {
            new Usuarios(1, "Admin", "admin", "password123", "Administrador"),
            new Usuarios(2, "Manager", "manager", "password123", "Cliente")
        };
    }

    public Usuarios ObtenerUsuario(string usuario, string contraseña)
    {
        return _usuarios.FirstOrDefault(u => u.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase) && u.Contraseña.Equals(contraseña));
    }
}
