using System;
using System.Text.Json.Serialization;
namespace EspacioTp5;
public class Productos
{
    public Productos(){}
    public Productos(string descripcion, int precio){
        Descripcion = descripcion;
        Precio = precio;
    }
    public Productos(int idProducto, string descripcion, int precio){
        IdProducto = idProducto;
        Descripcion = descripcion ?? string.Empty; 
        Precio = precio;
    }
    public int IdProducto { get; private set; }
    public string Descripcion { get;  private set; }
    public int Precio { get; private set; }
}
