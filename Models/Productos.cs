/*Productos
○ int idProducto
○ string descripcion
○ int precio*/
using System;
using System.Text.Json.Serialization;
namespace EspacioTp5;
public class Productos
{
    public Productos(){}
    public Productos(int idProducto, string descripcion, int precio)
    {
        IdProducto = idProducto;
        Descripcion = descripcion;
        Precio = precio;
    }
    [JsonPropertyName("idProducto")]
    public int IdProducto {get;private set;}
    [JsonPropertyName("descripcion")]
    public string Descripcion{get;private set;}
    [JsonPropertyName("precio")]
    public int Precio {get;private set;}

}
