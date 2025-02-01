using System;

namespace EspacioTp5
{
    public class Clientes
    {
        public int IdCliente { get; private set; }
        public string Nombre { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }

        // Constructor para nuevo cliente (sin ID aún)
        public Clientes(string nombre, string email, string telefono)
        {
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
        }

        // Constructor con ID (para modificar o cargar desde base de datos)
        public Clientes(int idCliente, string nombre, string email, string telefono)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
        }
    }
}