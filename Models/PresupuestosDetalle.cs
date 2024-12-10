using System;
using System.Text.Json.Serialization;
namespace EspacioTp5;

public class PresupuestosDetalle
{
    public PresupuestosDetalle(){
        Cantidad=0;
        Producto=new Productos();
    }
    public PresupuestosDetalle(Productos producto, int cantidad)
    {
        Producto = producto;
        Cantidad = cantidad;
    }
    [JsonPropertyName("producto")]
    public Productos Producto {get;private set;}
    [JsonPropertyName("cantidad")]
    public int Cantidad{get;private set;}
}
