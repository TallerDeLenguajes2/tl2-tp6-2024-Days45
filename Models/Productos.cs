using System.ComponentModel.DataAnnotations;

namespace EspacioTp5
{
    public class Productos
    {
        public Productos() { }

        public Productos(string descripcion, decimal precio)
        {
            Descripcion = descripcion;
            Precio = precio;
        }

        public Productos(int idProducto, string descripcion, decimal precio)
        {
            IdProducto = idProducto;
            Descripcion = descripcion ?? string.Empty;
            Precio = precio;
        }

        [Key]
        public int IdProducto { get; private set; }

        // Descripción opcional con longitud máxima de 250 caracteres
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string Descripcion { get; private set; }

        // Precio requerido y debe ser mayor que 0
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; private set; }
    }
}
