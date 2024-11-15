/*
PresupuestosDetalle
○ Productos producto
○ int cantidad
*/
using System;
using System.Text.Json.Serialization;
namespace EspacioTp5;

public class PresupuestosDetalle
{
    
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
