using System;
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
    public Productos Producto {get;private set;}
    public int Cantidad{get;private set;}
}
