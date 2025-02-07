using System;
namespace EspacioTp5;
public class Usuarios
{
    public int IdUsuario { get; private set; }
    public string Nombre { get; private set; }
    public string Usuario { get; private set; }
    public string Contraseña { get; private set; }
    public string Rol { get; private set; }
    public Clientes Cliente { get; private set; }  

    public Usuarios(string nombre, string usuarioNombre, string contraseña, string rol, Clientes cliente = null)
    {
        Nombre = nombre;
        Usuario = usuarioNombre;
        Contraseña = contraseña;
        Rol = rol;
        Cliente = cliente;  
    }

    public Usuarios(int id, string nombre, string usuarioNombre, string contraseña, string rol, Clientes cliente = null)
    {
        IdUsuario = id;
        Nombre = nombre;
        Usuario = usuarioNombre;
        Contraseña = contraseña;
        Rol = rol;
        Cliente = cliente;  
    }
}

