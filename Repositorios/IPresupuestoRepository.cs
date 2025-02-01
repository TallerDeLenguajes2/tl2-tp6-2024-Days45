using EspacioTp5;
using System.Collections.Generic;
using tl2_tp6_2024_Days45.ViewModel;
namespace rapositoriosTP5
{
    public interface IPresupuestoRepository
    {
        public void CrearPresupuesto(Presupuestos presupuesto);
        public List<Presupuestos> ListarPresupuestos();
        public Presupuestos ObtenerPresupuesto(int id);
        public void AgregarProductoAPresupuesto(int presupuestoId, Productos producto, int cantidad);
        public void EliminarPresupuesto(int id);
        public void ModificarPresupuesto(Presupuestos presupuesto, List<DetalleModificacionViewModel> detalles);
        
    }
}