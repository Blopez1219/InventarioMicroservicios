namespace TransaccionesService.Models
{
    public class ProductoDTO
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
    }
}
