
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBiblioteca.Shared.Models
{
    public class Libro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del libro es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreLibro { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(50, ErrorMessage = "El autor no puede superar los 50 caracteres.")]
        public string Autor { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "El número de páginas debe ser mayor que cero.")]
        public int NumPaginas { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria.")]
        [Column(TypeName = "date")]
        public DateTime? FechaPublicacion { get; set; }
    }
}
