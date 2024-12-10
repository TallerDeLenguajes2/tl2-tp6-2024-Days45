using System;
namespace EspacioTp5;
public class Usuarios
{
    public int Id { get; private set; }  
    public string Nombre { get; private set; }
    public string Usuario { get; private set; }  
    public string Contraseña { get; private set; }
    public string Rol { get; private set; }
    public Usuarios(string nombre, string usuarioNombre, string contraseña, string rol)
    {
        Nombre = nombre;
        Usuario = usuarioNombre;
        Contraseña = contraseña;
        Rol = rol;
    }
    public Usuarios(int id, string nombre, string usuarioNombre, string contraseña, string rol)
    {
        Id = id;
        Nombre = nombre;
        Usuario = usuarioNombre;
        Contraseña = contraseña;
        Rol = rol;
    }
}
