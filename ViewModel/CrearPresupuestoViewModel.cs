using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EspacioTp5;

namespace tl2_tp6_2024_Days45.ViewModel
{
    public class CrearPresupuestoViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public int IdCliente { get; set; }

        public List<Clientes> Clientes { get; set; } = new List<Clientes>();
        public List<Productos> Productos { get; set; } = new List<Productos>();
        public List<ProductoSeleccionado> ProductosSeleccionados { get; set; } = new List<ProductoSeleccionado>();
    }

    public class ProductoSeleccionado
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
