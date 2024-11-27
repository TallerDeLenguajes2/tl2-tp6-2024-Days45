using System.ComponentModel.DataAnnotations;

namespace EspacioTp5
{
    public class Clientes
    {
        public Clientes(int idCliente, string nombre, string email, string telefono)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
        }

        public Clientes(string nombre, string email, string telefono)
        {
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
        }

        public Clientes() { }

        [Key]
        public int IdCliente { get; private set; }

        // Nombre obligatorio
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; private set; }

        // Email con validación de tipo email
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico es incorrecto.")]
        public string Email { get; private set; }

        // Teléfono con validación de tipo teléfono (en este caso, solo números)
        [Phone(ErrorMessage = "El formato del teléfono es incorrecto.")]
        public string Telefono { get; private set; }
    }
}
