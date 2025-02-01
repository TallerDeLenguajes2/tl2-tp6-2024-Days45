using EspacioTp5;
using System.Collections.Generic;

namespace rapositoriosTP5
{
    public interface IClienteRepository
    {
        void CrearCliente(Clientes cliente);
        void ModificarCliente(int id, Clientes cliente);
        List<Clientes> ListarClientes();
        Clientes ObtenerCliente(int id);
        void EliminarCliente(int id);
    }
}