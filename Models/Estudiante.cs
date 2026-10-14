using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RegistroEstudiantes.Models
{
    public class Estudiante
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // [Unique] crea un índice único en SQLite: no permite dos estudiantes con la misma cuenta.
        [Unique, NotNull, MaxLength(20)]
        public string NumeroCuenta { get; set; } = string.Empty;

        [NotNull, MaxLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string Carrera { get; set; } = string.Empty;

        [NotNull, MaxLength(150)]
        public string Correo { get; set; } = string.Empty;
    }
}