using EspacioTp5;
using System.Collections.Generic;

namespace rapositoriosTP5
{
    public interface IProductoRepository
    {
        public void CrearProducto(Productos producto); 
        public void ModificarProducto(int id, Productos producto);
        public List<Productos> ListarProductos();
        public Productos ObtenerProducto(int id);
        public void EliminarProducto(int id);
    }
}
