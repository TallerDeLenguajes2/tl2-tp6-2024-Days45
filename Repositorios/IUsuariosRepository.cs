using EspacioTp5;
using System.Collections.Generic;

namespace rapositoriosTP5
{
    public interface IUsuariosRepository
    {
        public void CrearUsuario(Usuarios usuario);
        public List<Usuarios> ObtenerUsuarios();
        public Usuarios BuscarUsuarioPorId(int id);
        public void ModificarUsuario(Usuarios usuario);
        public void ModificarPassword(int id, string nuevaPassword);
        public void EliminarUsuario(int id);
        public void CambiarRol(int id, string nuevoRol);
    }
}
