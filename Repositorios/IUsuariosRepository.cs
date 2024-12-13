using EspacioTp5;
using System.Collections.Generic;

namespace rapositoriosTP5
{
    public interface IUsuariosRepository
    {
        Usuarios ObtenerUsuario(string usuario, string contraseña);
    }
}
