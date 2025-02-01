using System;
using System.Collections.Generic;
using System.Linq;

namespace EspacioTp5
{
    public class Presupuestos
    {
        public int IdPresupuesto { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public Clientes Cliente { get; private set; } 
        public List<PresupuestosDetalle> Detalle { get; private set; }

        // Constructor para nuevo presupuesto (sin ID aún)
        public Presupuestos(DateTime fechaCreacion, Clientes cliente, List<PresupuestosDetalle> detalle = null)
        {
            FechaCreacion = fechaCreacion;
            Cliente = cliente;
            Detalle = detalle ?? new List<PresupuestosDetalle>();
        }

        // Constructor con ID (para modificar o cargar desde base de datos)
        public Presupuestos(int idPresupuesto, DateTime fechaCreacion, Clientes cliente, List<PresupuestosDetalle> detalle = null)
        {
            IdPresupuesto = idPresupuesto;
            FechaCreacion = fechaCreacion;
            Cliente = cliente;
            Detalle = detalle ?? new List<PresupuestosDetalle>();
        }

        public double MontoPresupuesto()
        {
            double monto = 0;
            foreach (var item in Detalle)
            {
                monto += item.Producto.Precio * item.Cantidad;
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
            return Detalle.Sum(d => d.Cantidad); 
        }

        public void ModificarFecha(DateTime nuevaFecha)
        {
            this.FechaCreacion = nuevaFecha;
        }
    }
}