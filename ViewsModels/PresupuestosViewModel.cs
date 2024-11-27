using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EspacioTp5.ViewModels
{
    public class PresupuestosViewModel
    {
        public PresupuestosViewModel()
        {
            Clientes = new List<Clientes>();
        }

        // Propiedades del presupuesto
        public int IdPresupuesto { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Cliente seleccionado
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int IdClienteSeleccionado { get; set; }

        // Lista de clientes disponibles
        public List<Clientes> Clientes { get; set; }
    }
}
