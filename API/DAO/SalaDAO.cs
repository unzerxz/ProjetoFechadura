using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class SalaDAO
{
    private readonly MySqlConnection _connection;
    private static readonly Random Random = new Random();

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

    public int IsCredencialUsuarioTecladoValida(int credencialTeclado){
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


    public int IsCredencialUsuarioCartaoValida(string credencialCartao){
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

    public bool IsSalaAtiva(int idSala)
{
    try
    {
        _connection.Open();
        const string query = "SELECT isAtivo FROM bdFechadura.sala WHERE idSala = @IdSala";
        
        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@IdSala", idSala);
        
        var result = command.ExecuteScalar();
        
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToBoolean(result);
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

    return false; // Retorna false se houver um erro ou a sala não for encontrada
}

public bool IsSalaVazia(int idSala)
{
    try
    {
        _connection.Open();
        const string query = "SELECT status FROM bdFechadura.sala WHERE idSala = @IdSala";
        
        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@IdSala", idSala);
        
        var result = command.ExecuteScalar();
        
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToBoolean(result) == false; // Se o status for 0, significa que está vazia
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

    return false; // Retorna false se houver um erro ou a sala não for encontrada
}

public bool AbrirSala(int idSala, int idFuncionario)
    {
        try
        {
            _connection.Open();

            const string query = @"
                UPDATE bdFechadura.sala
                SET funcionario_idFuncionario = @IdFuncionario, status = 1
                WHERE idSala = @IdSala AND isAtivo = 1 AND status = 0;";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdSala", idSala);
            command.Parameters.AddWithValue("@IdFuncionario", idFuncionario);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0; // Retorna true se a sala foi aberta com sucesso
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido: {ex.Message}");
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

    public bool FecharSala(int idSala, int idFuncionario)
    {
        try
        {
            _connection.Open();

            const string query = @"
                UPDATE bdFechadura.sala
                SET status = 0
                WHERE idSala = @IdSala AND funcionario_idFuncionario = @IdFuncionario AND isAtivo = 1 AND status = 1;";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdSala", idSala);
            command.Parameters.AddWithValue("@IdFuncionario", idFuncionario);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0; // Retorna true se a sala foi fechada com sucesso
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro desconhecido: {ex.Message}");
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

    public string GenerateUniqueCredencialSala(string identificacaoSala) //Function used in the controller
    {
        if (identificacaoSala == null) throw new ArgumentNullException(nameof(identificacaoSala));

        string credencialSala;

        do
        {
            credencialSala = GenerateCredencialSala(identificacaoSala);
        }
        while (IsCredencialSalaExist(credencialSala));

        return credencialSala;
    }

    private string GenerateCredencialSala(string identificacaoSala)
    {
        // Static prefix
        string prefix = "792SN";

        // Identification of the room
        string identificacaoPart = identificacaoSala.Length > 0
            ? identificacaoSala.ToUpper()
            : "XXXX"; // Fallback if identificacaoSala is empty

        // Static "A_" part
        string staticPart = "A_";

        // Random characters (up to make the total length 20)
        int randomLength = 20 - (prefix.Length + identificacaoPart.Length + staticPart.Length);
        string randomPart = GenerateRandomString(randomLength);

        // Combine parts
        string credencialSala = $"{prefix}{identificacaoPart}{staticPart}{randomPart}";

        return credencialSala;
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[Random.Next(chars.Length)]);
        }
        return stringBuilder.ToString();
    }

    private bool IsCredencialSalaExist(string credencialSala)
    {
        bool exists = false;

        try
        {
            _connection.Open();
            const string query = "SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS credencial_status FROM sala WHERE credencialSala = @CredencialSala;";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@CredencialSala", credencialSala);

            exists = Convert.ToBoolean(command.ExecuteScalar());
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

        return exists;
    }

    public bool IsCredencialCorreta(int idSala, string credencial)
    {
        try
        {
            _connection.Open();
            const string query = "SELECT COUNT(1) FROM sala WHERE idSala = @IdSala AND credencialSala = @Credencial";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@IdSala", idSala);
            command.Parameters.AddWithValue("@Credencial", credencial);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erro do Banco: {ex.Message}");
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

}
