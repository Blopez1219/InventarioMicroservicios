using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosService.Data;
using ProductosService.Models;

namespace ProductosService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosDbContext _context;

        public ProductosController(ProductosDbContext context)
        {
            _context = context;
        }

        // 🔹 Obtener todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // 🔹 Obtener un producto por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return producto;
        }

        // 🔹 Crear un producto
        [HttpPost]
        public async Task<ActionResult<Producto>> CreateProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducto), new { id = producto.ID }, producto);
        }

        // 🔹 Actualizar un producto
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, Producto producto)
        {
            if (id != producto.ID) return BadRequest();

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProductoStock(int id, [FromBody] ProductoStockDTO productoStockDTO)
        {
            // Buscar el producto en la base de datos
            var productoExistente = await _context.Productos.FindAsync(id);

            if (productoExistente == null)
            {
                return NotFound();  // Si no existe el producto, retornar error 404
            }

            // Solo actualizar el campo Stock si el valor es diferente al actual
            if (productoStockDTO.Stock != productoExistente.Stock)
            {
                productoExistente.Stock = productoStockDTO.Stock;
            }

            // Marcar el producto como modificado y guardar cambios
            _context.Entry(productoExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Devolver respuesta exitosa sin contenido
            return NoContent();
        }


        // 🔹 Eliminar un producto
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos(
            [FromQuery] string nombre = null,
            [FromQuery] string categoria = null,
            [FromQuery] decimal? precioMin = null,
            [FromQuery] decimal? precioMax = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            IQueryable<Producto> query = _context.Productos;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => p.Categoria.Contains(categoria));
            }

            if (precioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= precioMax.Value);
            }

            // Paginación
            var productos = await query
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return Ok(productos);
        }

    }
}
