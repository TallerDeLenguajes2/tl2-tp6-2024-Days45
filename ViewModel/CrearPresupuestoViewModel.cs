using System;
using System.Collections.Generic;
using EspacioTp5;
using System.ComponentModel.DataAnnotations;
namespace tl2_tp6_2024_Days45.ViewModel
{
    public class CrearPresupuestoViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        public List<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
        public List<Productos> Productos { get; set; } = new List<Productos>();
        public List<ProductoSeleccionado> ProductosSeleccionados { get; set; } = new List<ProductoSeleccionado>();
    }


    public class ProductoSeleccionado
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }
}