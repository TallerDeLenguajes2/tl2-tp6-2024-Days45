/*
*Presupuestos
○ int IdPresupuesto
○ string nombreDestinatario
○ List<PresupuestoDetalle> detalle
○ Metodos
■ MontoPresupuesto ()
■ MontoPresupuestoConIva()
■ CantidadProductos ()
*/
using System;
using System.Text.Json.Serialization;
namespace EspacioTp5;

public class Presupuestos
{
    public Presupuestos()
    {
        Detalle = new List<PresupuestosDetalle>();
    }
    public Presupuestos(int idPresupuesto, string nombreDestinatario, DateTime fechaCreacion, List<PresupuestosDetalle> detalle = null)
    {
        IdPresupuesto = idPresupuesto;
        NombreDestinatario = nombreDestinatario;
        FechaCreacion = fechaCreacion;
        Detalle = detalle ?? new List<PresupuestosDetalle>();
    }
    public Presupuestos(string nombreDestinatario, DateTime fechaCreacion)
    {
        NombreDestinatario = nombreDestinatario;
        FechaCreacion = fechaCreacion;
    }
    public int IdPresupuesto { get; private set; }
    public string NombreDestinatario { get; private set; }
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
