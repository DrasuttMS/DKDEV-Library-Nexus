
using BlazorBiblioteca.Context;
using BlazorBiblioteca.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBiblioteca.Controller;

[ApiController]
[Route("api/[controller]")]
public class LibroController : ControllerBase
{
    private readonly LibroDBContext _context;

    public LibroController(LibroDBContext context)
    {
        _context = context;
    }

    // GET: api/Libro
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Libro>>> ObtenerTodos()
    {
        var libros = await _context.Libros
            .AsNoTracking()
            .OrderBy(libro => libro.Id)
            .ToListAsync();

        return Ok(libros);
    }

    // GET: api/Libro/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Libro>> ObtenerPorId(int id)
    {
        var libro = await _context.Libros
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (libro is null)
        {
            return NotFound(new { mensaje = "No se encontro el libro solicitado." });
        }

        return Ok(libro);
    }

    // POST: api/Libro
    [HttpPost]
    public async Task<ActionResult<Libro>> Agregar([FromBody] Libro libro)
    {
        libro.Id = 0;

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = libro.Id },
            libro);
    }

    // PUT: api/Libro/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Libro libro)
    {
        if (id != libro.Id)
        {
            return BadRequest(new
            {
                mensaje = "El identificador de la ruta no coincide con el libro."
            });
        }

        var libroExistente = await _context.Libros.FindAsync(id);

        if (libroExistente is null)
        {
            return NotFound(new
            {
                mensaje = "No se encontro el libro que desea actualizar."
            });
        }

        libroExistente.NombreLibro = libro.NombreLibro;
        libroExistente.Autor = libro.Autor;
        libroExistente.NumPaginas = libro.NumPaginas;
        libroExistente.FechaPublicacion = libro.FechaPublicacion;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Libro/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var libro = await _context.Libros.FindAsync(id);

        if (libro is null)
        {
            return NotFound(new
            {
                mensaje = "No se encontro el libro que desea eliminar."
            });
        }

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Libro/restablecer
    // Restaura la biblioteca a sus tres registros originales.
    [HttpPost("restablecer")]
    public async Task<IActionResult> RestablecerBiblioteca()
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Eliminar todos los registros actuales.
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM [dbo].[Libros];");

            // Reiniciar el identificador autoincremental.
            await _context.Database.ExecuteSqlRawAsync(
                "DBCC CHECKIDENT ('dbo.Libros', RESEED, 0);");

            // Recuperar los tres libros originales.
            var librosOriginales = new List<Libro>
            {
                new Libro
                {
                    NombreLibro = "El ojo del mundo",
                    Autor = "Robert Jordan",
                    NumPaginas = 824,
                    FechaPublicacion = new DateTime(1990, 1, 15)
                },
                new Libro
                {
                    NombreLibro = "Orgullo o prejuicio",
                    Autor = "Jane Austen",
                    NumPaginas = 500,
                    FechaPublicacion = new DateTime(1813, 1, 28)
                },
                new Libro
                {
                    NombreLibro = "Juego de tronos",
                    Autor = "George R.R. Martin",
                    NumPaginas = 800,
                    FechaPublicacion = new DateTime(1996, 8, 1)
                }
            };

            _context.Libros.AddRange(librosOriginales);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new
            {
                mensaje = "La biblioteca fue restablecida correctamente.",
                cantidad = librosOriginales.Count,
                libros = librosOriginales
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
