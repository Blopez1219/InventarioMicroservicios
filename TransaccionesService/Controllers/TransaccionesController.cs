using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using TransaccionesService.Data;
using TransaccionesService.Models;

namespace TransaccionesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly TransaccionesDbContext _context;
        private readonly HttpClient _httpClient;

        public TransaccionesController(TransaccionesDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // 🔹 Obtener todas las transacciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransacciones()
        {
            return await _context.Transacciones.ToListAsync();
        }

        // 🔹 Obtener una transacción por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaccion>> GetTransaccion(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();
            return transaccion;
        }

        // 🔹 Registrar una nueva transacción
        [HttpPost]
        public async Task<ActionResult<Transaccion>> CreateTransaccion(Transaccion transaccion)
        {
            // Hacer una solicitud HTTP GET al microservicio de productos para obtener el producto
            var response = await _httpClient.GetAsync($"http://localhost:5230/api/productos/{transaccion.ProductoID}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Producto no encontrado.");
            }

            var producto = await response.Content.ReadFromJsonAsync<ProductoDTO>();

            if (producto == null)
            {
                return BadRequest("Producto no encontrado.");
            }
            // ✅ Validar stock antes de registrar una venta
            if (transaccion.TipoTransaccion == "venta")
            {
                if (producto.Stock < transaccion.Cantidad)
                {
                    return BadRequest("Stock insuficiente.");
                }

                // Restar stock del producto
                producto.Stock -= transaccion.Cantidad;
            }
            else if (transaccion.TipoTransaccion == "compra")
            {
                // Aumentar stock del producto
                producto.Stock += transaccion.Cantidad;
            }

            var productoConNuevoStock = new ProductoStockDTO
            {
                Stock = producto.Stock  // Solo el campo Stock
            };

            // Actualizar el producto (es importante enviar esta información al microservicio de productos para que actualice el stock)
            var updateResponse = await _httpClient.PatchAsJsonAsync($"http://localhost:5230/api/productos/{producto.ID}", productoConNuevoStock);

            if (!updateResponse.IsSuccessStatusCode)
            {
                return BadRequest("Error al actualizar stock del producto.");
            }

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTransaccion), new { id = transaccion.ID }, transaccion);
        }

        // 🔹 Eliminar una transacción
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaccion(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 🔹 Historial de transacciones por producto (con filtros)
        [HttpGet("historial/{productoId}")]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetHistorialTransacciones(
            int productoId,
            [FromQuery] string tipo = null, 
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] string orden = "desc"
        )
        {
            // Obtén las transacciones para el producto
            IQueryable<Transaccion> query = _context.Transacciones.Where(t => t.ProductoID == productoId);

            // Filtro por tipo de transacción (compra o venta)
            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(t => t.TipoTransaccion.ToLower() == tipo.ToLower());
            }

            // Filtro por fecha de inicio y fin
            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.Fecha <= fechaFin.Value);
            }

            // Ordenar las transacciones por fecha
            if (orden.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(t => t.Fecha);
            }
            else
            {
                query = query.OrderByDescending(t => t.Fecha);
            }

            // Obtener el historial de transacciones
            var transacciones = await query.ToListAsync();

            // Verifica si se encontró el historial
            if (!transacciones.Any())
            {
                return NotFound(new { Message = "No se encontraron transacciones para este producto." });
            }

            return Ok(transacciones);
        }


        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Transaccion>>> BuscarTransacciones(
            [FromQuery] string tipo = null,
            [FromQuery] int? productoId = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            IQueryable<Transaccion> query = _context.Transacciones;

            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(t => t.TipoTransaccion.ToLower() == tipo.ToLower());
            }

            if (productoId.HasValue)
            {
                query = query.Where(t => t.ProductoID == productoId.Value);
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.Fecha <= fechaFin.Value);
            }

            // Paginación
            var transacciones = await query
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return Ok(transacciones);
        }

    }
}
