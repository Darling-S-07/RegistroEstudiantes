using SQLite;
using RegistroEstudiantes.Models;

namespace RegistroEstudiantes.Data;

/// Maneja la conexión e inicialización de la base de datos SQLite
/// y las operaciones sobre la tabla de productos.
public class SQLiteDatabase
{
    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    private readonly string _dbPath;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _database;

    public SQLiteDatabase(string dbPath)
    {
        _dbPath = dbPath;
    }

    // Abre la conexión y crea la tabla la primera vez que se necesita,
    // sin bloquear el hilo de la interfaz (sin .Wait()).
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database is not null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_database is null)
            {
                var connection = new SQLiteAsyncConnection(_dbPath, Flags);
                await connection.CreateTableAsync<Estudiante>();
                _database = connection;
            }

            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Create: guarda un producto. Devuelve el número de filas insertadas.
    // Tras insertar, producto.IdProducto contiene el ID generado.
    public async Task<int> GuardarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.InsertAsync(estudiante);
    }

    // Read
    public async Task<List<Estudiante>> ObtenerEstudianteAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Estudiante>().ToListAsync();
    }

    // Read by Id
    public async Task<Estudiante?> ObtenerEstudianteIdAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.FindAsync<Estudiante>(id);
    }

    // Update
    public async Task<int> ActualizarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.UpdateAsync(estudiante);
    }

    // Delete
    public async Task<int> EliminarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.DeleteAsync(estudiante);
    }
}
