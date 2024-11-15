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
    [JsonConstructor]
    public Presupuestos(int idPresupuesto, string nombreDestinatario, DateTime fechaCreacion, List<PresupuestosDetalle> detalle = null)
    {
        IdPresupuesto = idPresupuesto;
        NombreDestinatario = nombreDestinatario;
        FechaCreacion = fechaCreacion;  // Aquí se pasa la fecha al crear el presupuesto
        Detalle = detalle ?? new List<PresupuestosDetalle>();
    }
    [JsonPropertyName("idPresupuesto")]
    public int IdPresupuesto { get; private set; }
    [JsonPropertyName("nombreDestinatario")]
    public string NombreDestinatario { get; private set; }
    [JsonPropertyName("detalle")]
    public List<PresupuestosDetalle> Detalle { get; private set; }
    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; private set; } // Nueva propiedad

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
