using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class FuncionarioDAO
{
    private readonly MySqlConnection _connection;
    private static readonly Random Random = new Random();

    public FuncionarioDAO()
    {
        _connection = MySqlConnectionFactory.GetConnection();
    }

   private static List<Funcionario?> ReadAll(MySqlCommand command)
{
    var funcionarios = new List<Funcionario?>();

    using var reader = command.ExecuteReader();
    if (!reader.HasRows) return funcionarios;
    while (reader.Read())
    {
        var funcionario = new Funcionario
        {
            IdFuncionario = reader.IsDBNull(reader.GetOrdinal("idFuncionario")) ? 0 : reader.GetInt32("idFuncionario"),
            Nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? string.Empty : reader.GetString("nome"),
            NomeUsuario = reader.IsDBNull(reader.GetOrdinal("nomeUsuario")) ? string.Empty : reader.GetString("nomeUsuario"),
            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email"),
            CredencialCartao = reader.IsDBNull(reader.GetOrdinal("credencialCartao")) ? string.Empty : reader.GetString("credencialCartao"),
            CredencialTeclado = reader.IsDBNull(reader.GetOrdinal("credencialTeclado")) ? 0 : reader.GetInt32("credencialTeclado"),
            Senha = reader.IsDBNull(reader.GetOrdinal("senha")) ? string.Empty : reader.GetString("senha"),
            Cargo_IdCargo = reader.IsDBNull(reader.GetOrdinal("cargo_idCargo")) ? 0 : reader.GetInt32("cargo_idCargo"),
            Perfil_IdPerfil = reader.IsDBNull(reader.GetOrdinal("perfil_idPerfil")) ? 0 : reader.GetInt32("perfil_idPerfil"),
            IsAtivo = reader.IsDBNull(reader.GetOrdinal("isAtivo")) ? 0 : reader.GetInt32("isAtivo")
        };
        funcionarios.Add(funcionario);
    }

    return funcionarios;
}



    public List<Funcionario?> Read()
    {
        List<Funcionario?>? funcionarios = null;

        try
        {
            _connection.Open();
            const string query = "SELECT * FROM funcionario";

            var command = new MySqlCommand(query, _connection);

            funcionarios = ReadAll(command);
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

        return funcionarios;

    }

    public Funcionario? ReadById(int id)
    {
        Funcionario? funcionario = null;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.funcionario WHERE idFuncionario = @Id";

            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            funcionario = ReadAll(command).FirstOrDefault();
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

        return funcionario;
    }

   public int Create(Funcionario funcionario)
{
    int id = 0;
    try
    {
        _connection.Open();
        const string query = "INSERT INTO funcionario (nome, nomeUsuario, email, credencialCartao, credencialTeclado, senha, cargo_idCargo, perfil_idPerfil, isAtivo) " +
                 "VALUES (@Nome, @NomeUsuario, @Email, @CredencialCartao, @CredencialTeclado, @Senha, 1, 1, 0)";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@Nome", funcionario.Nome);
        command.Parameters.AddWithValue("@NomeUsuario", funcionario.NomeUsuario);
        command.Parameters.AddWithValue("@Email", funcionario.Email);
        command.Parameters.AddWithValue("@CredencialCartao", DBNull.Value); // Correção para valores nulos
        command.Parameters.AddWithValue("@CredencialTeclado", DBNull.Value); // Correção para valores nulos
        command.Parameters.AddWithValue("@Senha", funcionario.Senha);
        command.Parameters.AddWithValue("@CargoId", 1); // Ajuste conforme o ID válido
        command.Parameters.AddWithValue("@PerfilId", 1); // Ajuste conforme o ID válido
        command.Parameters.AddWithValue("@IsAtivo", 0); // Correção para o valor booleano

        command.ExecuteNonQuery();
        id = (int)command.LastInsertedId;
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
    return id;
}

    public void Update(int id, Funcionario funcionario)
{
    try
    {
        _connection.Open();
        const string query = "UPDATE funcionario SET " +
                 "nome = @Nome, " +
                 "nomeUsuario = @NomeUsuario, " +
                 "email = @Email, " +
                 "credencialCartao = @CredencialCartao, " +
                 "credencialTeclado = @CredencialTeclado, " +
                 "senha = @Senha, " +
                 "cargo_idCargo = @CargoId, " +
                 "perfil_idPerfil = @PerfilId, " +
                 "isAtivo = @IsAtivo " +
                 "WHERE idFuncionario = @IdFuncionario";
        using var command = new MySqlCommand(query, _connection);

        command.Parameters.AddWithValue("@Nome", funcionario.Nome);
        command.Parameters.AddWithValue("@NomeUsuario", funcionario.NomeUsuario);
        command.Parameters.AddWithValue("@Email", funcionario.Email);
        command.Parameters.AddWithValue("@CredencialCartao", funcionario.CredencialCartao);
        command.Parameters.AddWithValue("@CredencialTeclado", funcionario.CredencialTeclado);
        command.Parameters.AddWithValue("@Senha", funcionario.Senha);
        command.Parameters.AddWithValue("@CargoId", funcionario.Cargo_IdCargo);
        command.Parameters.AddWithValue("@PerfilId", funcionario.Perfil_IdPerfil);
        command.Parameters.AddWithValue("@IsAtivo", funcionario.IsAtivo);
        command.Parameters.AddWithValue("@IdFuncionario", id);

        command.ExecuteNonQuery();

    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Erro do Banco: {ex.Message} ");
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


    public void Delete(int idFuncionario)
    {
        try
        {
            _connection.Open();
            const string query = "DELETE FROM funcionario WHERE idFuncionario = @IdFuncionario";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdFuncionario", idFuncionario);
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

public bool IsCredencialPasswordExist(int credencialTeclado)
{
    bool exists = false;

    try
    {
        _connection.Open();
        const string query = "SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS credencial_status FROM funcionario WHERE credencialTeclado = @CredencialTeclado;";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@CredencialTeclado", credencialTeclado);

        // Execute the query and get the result
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

public bool IsCredencialCartaoExist(string credencialCartao)
{
    bool exists = false;

    try
    {
        _connection.Open();
        const string query = "SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS credencial_status FROM funcionario WHERE credencialCartao = @CredencialCartao;";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@CredencialCartao", credencialCartao);

        // Execute the query and get the result
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

public static string GenerateCredencialCartao(Funcionario funcionario)
    {
        if (funcionario == null) throw new ArgumentNullException(nameof(funcionario));

        // Static prefix
        string prefix = "SN792";

        // First 3 letters of the employee's name
        string nomePart = funcionario.Nome.Length >= 3 
            ? funcionario.Nome.Substring(0, 3).ToUpper() 
            : funcionario.Nome.ToUpper().PadRight(3, 'X'); // Fallback if name is shorter than 3 characters

        // Perfil ID
        string perfilPart = funcionario.Perfil_IdPerfil.ToString();

        // Random 10 characters
        string randomPart = GenerateRandomString(10);

        // Combine all parts
        string[] parts = { prefix, nomePart, perfilPart, "D" + randomPart };
        string shuffledCredencialCartao = ShuffleParts(parts);

        return shuffledCredencialCartao;
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[Random.Next(chars.Length)]);
        }
        return stringBuilder.ToString();
    }

    private static string ShuffleParts(string[] parts)
    {
        var random = new Random();
        // Shuffle the array of parts
        parts = parts.OrderBy(x => random.Next()).ToArray();
        // Join the shuffled parts into a single string
        return string.Join("", parts);
    }

    public static string GenerateRandomPassword()
    {
        const string digits = "0123456789";
        var password = new char[6];

        for (int i = 0; i < 6; i++)
        {
            password[i] = digits[Random.Next(digits.Length)];
        }

        return new string(password);
    }

        public string GenerateUniqueRandomPassword()//Function used in the controller
    {
        string password;

        do
        {
            password = GenerateRandomPassword();
        }
        while (IsCredencialPasswordExist(int.Parse(password)));

        return password;
    }

        public string GenerateUniqueCredencialCartao(Funcionario funcionario)//Function used in the controller
    {
        if (funcionario == null) throw new ArgumentNullException(nameof(funcionario));

        string credencialCartao;

        do
        {
            credencialCartao = GenerateCredencialCartao(funcionario);
        }
        while (IsCredencialCartaoExist(credencialCartao));

        return credencialCartao;
    }

    public bool IsFuncionarioAtivo(int idFuncionario)
{
    bool isActive = false;

    try
    {
        _connection.Open();
        const string query = "SELECT isAtivo FROM funcionario WHERE idFuncionario = @IdFuncionario";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@IdFuncionario", idFuncionario);

        var result = command.ExecuteScalar();
        if (result != null && Convert.ToBoolean(result) == true)
        {
            isActive = true;
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

    return isActive;
}

public int AuthenticateUser(string identifier, string senha)
{
    int idFuncionario = 0;

    try
    {
        _connection.Open();
        const string query = "SELECT idFuncionario FROM funcionario WHERE (nomeUsuario = @Identifier OR email = @Identifier) AND senha = @Senha";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@Identifier", identifier);
        command.Parameters.AddWithValue("@Senha", senha);

        var result = command.ExecuteScalar();
        if (result != null)
        {
            idFuncionario = Convert.ToInt32(result);
            Console.WriteLine($"ID do funcionário autenticado no DAO: {idFuncionario}");
        }
        else
        {
            Console.WriteLine("Nenhum funcionário encontrado com as credenciais fornecidas");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na autenticação: {ex.Message}");
    }
    finally
    {
        _connection.Close();
    }

    return idFuncionario;
}

public bool IsFuncionarioAdmin(int idFuncionario)
{
    try
    {
        _connection.Open();
        const string query = "SELECT COUNT(*) > 0 FROM funcionario WHERE idFuncionario = @IdFuncionario AND perfil_idPerfil = 3 AND isAtivo = 1";

        using var command = new MySqlCommand(query, _connection);
        command.Parameters.AddWithValue("@IdFuncionario", idFuncionario);

        var result = Convert.ToBoolean(command.ExecuteScalar());
        Console.WriteLine($"IsFuncionarioAdmin para ID {idFuncionario}: {result}");
        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao verificar se funcionário é admin: {ex.Message}");
        return false;
    }
    finally
    {
        if (_connection.State == System.Data.ConnectionState.Open)
        {
            _connection.Close();
        }
    }
}

    internal object GetJwtSecret()
    {
        throw new NotImplementedException();
    }
}