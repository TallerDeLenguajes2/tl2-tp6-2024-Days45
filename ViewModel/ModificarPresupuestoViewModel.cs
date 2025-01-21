public class ModificarPresupuestoViewModel
{
    public int IdPresupuesto { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<DetalleModificacionViewModel> Detalles { get; set; }
}

public class DetalleModificacionViewModel
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; }
    public int Cantidad { get; set; }
    public int Precio { get; set; }
}
