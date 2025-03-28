namespace TransaccionesService.Models
{
    public class Transaccion
    {
        public int ID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string TipoTransaccion { get; set; } = string.Empty; // "compra" o "venta"
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal => Cantidad * PrecioUnitario;
        public string Detalle { get; set; } = string.Empty;
    }
}
