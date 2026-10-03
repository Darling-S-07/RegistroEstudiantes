using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RegistroEstudiantes.Models
{
    public class Estudiante
    {
        [PrimaryKey, AutoIncrement]
        public int IdEstudiante { get; set; }

        [MaxLength(100), NotNull]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100), NotNull]
        public string Apellido { get; set; } = string.Empty;

        [MaxLength(150), NotNull]
        public string Correo { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Carrera { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
