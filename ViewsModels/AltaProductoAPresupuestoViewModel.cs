using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EspacioTp5.ViewModels
{
    public class AltaProductoAPresupuestoViewModel
    {
        public AltaProductoAPresupuestoViewModel()
        {
            Productos = new List<Productos>();
        }

        // Detalles del producto a agregar
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int IdProductoSeleccionado { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        // Lista de productos disponibles
        public List<Productos> Productos { get; set; }
    }
}
