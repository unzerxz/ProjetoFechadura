using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class CargoDAO
{
    private readonly MySqlConnection _connection;

    public CargoDAO()
    {
        _connection = MySqlConnectionFactory.GetConnection();
    }

    private static List<Cargo?> ReadAll(MySqlCommand command)
    {
        var cargos = new List<Cargo?>();

        using var reader = command.ExecuteReader();
        if (!reader.HasRows) return cargos;
        while (reader.Read())
        {
            var cargo = new Cargo
            {
                IdCargo = reader.GetInt32("idCargo"),
                NomeCargo = reader.GetString("nomeCargo")
            };
            cargos.Add(cargo);
        }

        return cargos;
    }

    public List<Cargo?> Read()
    {
        List<Cargo?> cargos = null!;

        try
        {
            _connection.Open();
            const string query = "SELECT * FROM cargo";

            var command = new MySqlCommand(query, _connection);

            cargos = ReadAll(command);
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido{ex.Message}");
        }
        finally
        {
            _connection.Close();
        }

        return cargos;
    }

    public Cargo? ReadById(int id)
    {
        Cargo? cargo = null!;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.cargo WHERE idCargo = @Id";

            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            cargo = ReadAll(command).FirstOrDefault();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido{ex.Message}");
        }
        finally
        {
            _connection.Close();
        }

        return cargo;
    }

    public int Create(Cargo cargo)
    {
        int id = 0;
        try
        {
            _connection.Open();
            const string query = "INSERT INTO cargo (nomeCargo) " +
                                 "VALUES (@NomeCargo)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@NomeCargo", cargo.NomeCargo);

            command.ExecuteNonQuery();
            id = (int)command.LastInsertedId;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido{ex.Message}");
        }
        finally
        {
            _connection.Close();
        }
        return id;
    }

    public void Update(int id, Cargo cargo)
    {
        try
        {
            _connection.Open();
            const string query = "UPDATE cargo SET " +
                                 "nomeCargo = @NomeCargo " +
                                 "WHERE idCargo = @IdCargo";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@NomeCargo", cargo.NomeCargo);
            command.Parameters.AddWithValue("@IdCargo", id);

            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido{ex.Message}");
        }
        finally
        {
            _connection.Close();
        }
    }

    public void Delete(int idCargo)
    {
        try
        {
            _connection.Open();
            const string query = "DELETE FROM cargo WHERE idCargo = @IdCargo";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdCargo", idCargo);
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido: {ex.Message}");
        }
        finally
        {
            _connection.Close();
        }
    }
}
