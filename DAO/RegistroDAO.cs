using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class RegistroDAO
{
    private readonly MySqlConnection _connection;

    public RegistroDAO()
    {
        _connection = MySqlConnectionFactory.GetConnection();
    }

    private static List<Registro?> ReadAll(MySqlCommand command)
    {
        var registros = new List<Registro?>();

        using var reader = command.ExecuteReader();
        if (!reader.HasRows) return registros;
        while (reader.Read())
        {
            var registro = new Registro
            {
                IdRegistro = reader.GetInt32("idRegistro"),
                HorarioEntrada = reader.GetDateTime("horarioEntrada"),
                HorarioSaida = reader.IsDBNull(reader.GetOrdinal("horarioSaida")) ? (DateTime?)null : reader.GetDateTime("horarioSaida"),
                Sala_IdSala = reader.GetInt32("sala_idSala"),
                Funcionario_IdFuncionario = reader.GetInt32("funcionario_idFuncionario")
            };
            registros.Add(registro);
        }

        return registros;
    }

    public List<Registro?> Read()
    {
        List<Registro?> registros = null!;

        try
        {
            _connection.Open();
            const string query = "SELECT * FROM registro";

            var command = new MySqlCommand(query, _connection);

            registros = ReadAll(command);
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

        return registros;
    }

    public Registro? ReadById(int id)
    {
        Registro? registro = null!;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.registro WHERE idRegistro = @Id";

            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            registro = ReadAll(command).FirstOrDefault();
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

        return registro;
    }

    public int Create(Registro registro)
    {
        int id = 0;
        try
        {
            _connection.Open();
            const string query = "INSERT INTO registro (horarioEntrada, horarioSaida, sala_idSala, funcionario_idFuncionario) " +
                                 "VALUES (@HorarioEntrada, @HorarioSaida, @SalaId, @FuncionarioId)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@HorarioEntrada", registro.HorarioEntrada);
            command.Parameters.AddWithValue("@HorarioSaida", registro.HorarioSaida);
            command.Parameters.AddWithValue("@SalaId", registro.Sala_IdSala);
            command.Parameters.AddWithValue("@FuncionarioId", registro.Funcionario_IdFuncionario);

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

    public void Update(int id, Registro registro)
    {
        try
        {
            _connection.Open();
            const string query = "UPDATE registro SET " +
                                 "horarioEntrada = @HorarioEntrada, " +
                                 "horarioSaida = @HorarioSaida, " +
                                 "sala_idSala = @SalaId, " +
                                 "funcionario_idFuncionario = @FuncionarioId " +
                                 "WHERE idRegistro = @IdRegistro";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@HorarioEntrada", registro.HorarioEntrada);
            command.Parameters.AddWithValue("@HorarioSaida", registro.HorarioSaida);
            command.Parameters.AddWithValue("@SalaId", registro.Sala_IdSala);
            command.Parameters.AddWithValue("@FuncionarioId", registro.Funcionario_IdFuncionario);
            command.Parameters.AddWithValue("@IdRegistro", id);

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

    public void Delete(int idRegistro)
    {
        try
        {
            _connection.Open();
            const string query = "DELETE FROM registro WHERE idRegistro = @IdRegistro";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdRegistro", idRegistro);
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
