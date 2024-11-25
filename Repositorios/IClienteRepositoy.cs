using EspacioTp5;
using System.Collections.Generic;
namespace rapositoriosTP5
{
    public interface IClienteRepository
    {
        // Crear un cliente
        void CrearCliente(Clientes cliente);

        // Obtener un cliente por ID
        Clientes ObtenerCliente(int id);

        // Modificar un cliente por ID
        void ModificarCliente(int id, Clientes cliente);

        // Eliminar un cliente por ID
        void EliminarCliente(int id);

        // Listar todos los clientes
        List<Clientes> ListarClientes();
    }
}
