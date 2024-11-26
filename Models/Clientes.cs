using System;

namespace EspacioTp5;

public class Clientes
{
    public Clientes(int idCliente, string nombre, string email, string telefono)
    {
        IdCliente = idCliente;
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    public Clientes(string nombre, string email, string telefono)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    public Clientes() { }

    public int IdCliente { get; private set; }
    public string Nombre { get; private set; }
    public string Email { get; private set; }
    public string Telefono { get; private set; }

    public static void MostrarClientes(List<Clientes> clientes)
    {
        Console.WriteLine("Lista de Clientes:");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"ID: {cliente.IdCliente}, Nombre: {cliente.Nombre}");
        }
    }
}
