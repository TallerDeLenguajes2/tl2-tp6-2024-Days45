using System;
using System.Collections.Generic;

namespace EspacioTp5
{
    public class Presupuestos
    {
        public int IdPresupuesto { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public Usuarios Usuario { get; private set; }  
        public List<PresupuestosDetalle> Detalle { get; private set; }

        // Constructor para nuevo presupuesto (sin ID aún)
        public Presupuestos(DateTime fechaCreacion, Usuarios usuario, List<PresupuestosDetalle> detalle = null)
        {
            FechaCreacion = fechaCreacion;
            Usuario = usuario;
            Detalle = detalle ?? new List<PresupuestosDetalle>();
        }

        // Constructor con ID (para modificar o cargar desde base de datos)
        public Presupuestos(int idPresupuesto, DateTime fechaCreacion, Usuarios usuario, List<PresupuestosDetalle> detalle = null)
        {
            IdPresupuesto = idPresupuesto;
            FechaCreacion = fechaCreacion;
            Usuario = usuario;
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
            return Detalle.Count;
        }
    }
}
