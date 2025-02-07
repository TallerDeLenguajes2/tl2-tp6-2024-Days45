using EspacioTp5;
using System.Collections.Generic;

namespace rapositoriosTP5
{
    public interface IUsuariosRepository
    {
        public Usuarios ObtenerUsuario(string usuario, string contraseña);
        public Usuarios ObtenerUsuarioPorId(int id);
        public List<Usuarios> ListarUsuarios();
        public int? ObtenerIdClientePorUsuario(int idUsuario);
    }
}
