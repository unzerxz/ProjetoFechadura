using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class SalaDAO
{
    private readonly MySqlConnection _connection;

    public SalaDAO()
    {
        _connection = MySqlConnectionFactory.GetConnection();
    }

    private static List<Sala?> ReadAll(MySqlCommand command)
    {
        var salas = new List<Sala?>();

        using var reader = command.ExecuteReader();
        if (!reader.HasRows) return salas;
        while (reader.Read())
        {
            var sala = new Sala
            {
                IdSala = reader.GetInt32("idSala"),
                IdentificacaoSala = reader.GetString("identificacaoSala"),
                Status = reader.GetInt32("status"),
                CredencialSala = reader.IsDBNull(reader.GetOrdinal("credencialSala")) ? null : reader.GetString("credencialSala"),
                IsAtivo = reader.GetInt32("isAtivo"),
                Funcionario_IdFuncionario = reader.GetInt32("funcionario_idFuncionario")
            };
            salas.Add(sala);
        }

        return salas;
    }

    public List<Sala?> Read()
    {
        List<Sala?> salas = null!;

        try
        {
            _connection.Open();
            const string query = "SELECT * FROM sala";

            var command = new MySqlCommand(query, _connection);

            salas = ReadAll(command);
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

        return salas;
    }

    public Sala? ReadById(int id)
    {
        Sala? sala = null!;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.sala WHERE idSala = @Id";

            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            sala = ReadAll(command).FirstOrDefault();
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

        return sala;
    }

    public int Create(Sala sala)
    {
        int id = 0;
        try
        {
            _connection.Open();
            const string query = "INSERT INTO sala (identificacaoSala, status, credencialSala, isAtivo, funcionario_idFuncionario) " +
                                 "VALUES (@IdentificacaoSala, @Status, @CredencialSala, @IsAtivo, @FuncionarioId)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdentificacaoSala", sala.IdentificacaoSala);
            command.Parameters.AddWithValue("@Status", sala.Status);
            command.Parameters.AddWithValue("@CredencialSala", sala.CredencialSala);
            command.Parameters.AddWithValue("@IsAtivo", sala.IsAtivo);
            command.Parameters.AddWithValue("@FuncionarioId", sala.Funcionario_IdFuncionario);

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

    public void Update(int id, Sala sala)
    {
        try
        {
            _connection.Open();
            const string query = "UPDATE sala SET " +
                                 "identificacaoSala = @IdentificacaoSala, " +
                                 "status = @Status, " +
                                 "credencialSala = @CredencialSala, " +
                                 "isAtivo = @IsAtivo, " +
                                 "funcionario_idFuncionario = @FuncionarioId " +
                                 "WHERE idSala = @IdSala";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdentificacaoSala", sala.IdentificacaoSala);
            command.Parameters.AddWithValue("@Status", sala.Status);
            command.Parameters.AddWithValue("@CredencialSala", sala.CredencialSala);
            command.Parameters.AddWithValue("@IsAtivo", sala.IsAtivo);
            command.Parameters.AddWithValue("@FuncionarioId", sala.Funcionario_IdFuncionario);
            command.Parameters.AddWithValue("@IdSala", id);

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

    public void Delete(int idSala)
    {
        try
        {
            _connection.Open();
            const string query = "DELETE FROM sala WHERE idSala = @IdSala";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdSala", idSala);
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

    public int IsCredencialUsuarioTecladoValida(int credencialTeclado)
{
    int idFuncionario = -1;

    try
    {
        _connection.Open();

        const string query = "SELECT idFuncionario FROM bdFechadura.funcionario WHERE credencialTeclado = @CredencialTeclado;";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@CredencialTeclado", credencialTeclado);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            idFuncionario = reader.GetInt32("idFuncionario");
        }
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

    return idFuncionario;
}


        public int IsCredencialUsuarioCartaoValida(string credencialCartao)
    {
        int idFuncionario = -1;

        try
        {
            _connection.Open();

            const string query = "SELECT idFuncionario FROM bdFechadura.funcionario WHERE credencialCartao = @CredencialCartao;";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@CredencialCartao", credencialCartao);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                idFuncionario = reader.GetInt32("idFuncionario");
            }
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

        return idFuncionario;
    }

}
