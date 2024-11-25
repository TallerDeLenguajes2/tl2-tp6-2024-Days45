using System;
namespace EspacioTp5;
public class Presupuestos
{
    public Presupuestos(){
        Detalle = new List<PresupuestosDetalle>();
    }
    public Presupuestos(int idPresupuesto,Clientes cliente,List<PresupuestosDetalle> detalle,DateTime fechaCreacion){
        IdPresupuesto = idPresupuesto;
        Cliente = cliente;
        Detalle = detalle;
        FechaCreacion = fechaCreacion;
    }
    public Presupuestos(Clientes cliente, List<PresupuestosDetalle> detalle, DateTime fechaCreacion)
    {
        Cliente = cliente;
        Detalle = detalle;
        FechaCreacion = fechaCreacion;
    }

    public int IdPresupuesto { get; private set; }
    public Clientes Cliente{ get; private set; }
    public List<PresupuestosDetalle> Detalle { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public double MontoPresupuesto()
    {
        double monto = 0.0;
        foreach (var item in Detalle)
        {
            monto += (item.Producto.Precio * item.Cantidad);
        }
        return monto;
    }
    public double MontoPresupuestoConIva()
    {
        const double IVA = 0.21;
        return MontoPresupuesto() * (1 + IVA);
    }
    public int CantidadProductos()
    {
        return Detalle.Count();
    }
}
