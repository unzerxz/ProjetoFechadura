using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class FuncionarioDao
{
    private readonly MySqlConnection _connection;

    public FuncionarioDao()
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
                IdFuncionario = reader.GetInt32("idFuncionario"),
                Nome = reader.GetString("nome"),
                NomeUsuario = reader.GetString("nomeUsuario"),
                CredencialCartao = reader.GetString("credencialCartao"),
                CredencialTeclado = reader.GetInt32("credencialTeclado"),
                Senha = reader.GetString("senha"),
                Cargo_IdCargo = reader.GetInt32("cargo_idCargo"),
                Perfil_IdPerfil = reader.GetInt32("perfil_idPerfil")
            };
            funcionarios.Add(funcionario);
        }

        return funcionarios;
    }

    public List<Funcionario?> Read()
    {
        List<Funcionario?> funcionarios = null!;

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
        Funcionario? funcionario = null!;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.funcionario Where idFuncionario = @Id";

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
            const string query = "INSERT INTO funcionario (nome, nomeUsuario, credencialCartao, credencialTeclado, senha, cargo_idCargo, perfil_idPerfil) " +
                     "VALUES (@Nome, @NomeUsuario, @CredencialCartao, @CredencialTeclado, @Senha, @CargoId, @PerfilId)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Nome", funcionario.Nome);
            command.Parameters.AddWithValue("@NomeUsuario", funcionario.NomeUsuario);
            command.Parameters.AddWithValue("@CredencialCartao", funcionario.CredencialCartao);
            command.Parameters.AddWithValue("@CredencialTeclado", funcionario.CredencialTeclado);
            command.Parameters.AddWithValue("@Senha", funcionario.Senha);
            command.Parameters.AddWithValue("@CargoId", funcionario.Cargo_IdCargo);
            command.Parameters.AddWithValue("@PerfilId", funcionario.Perfil_IdPerfil);

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

    public void Update(int id, Funcionario funcionario)
    {
        try
        {
            _connection.Open();
            const string query = "UPDATE funcionario SET " +
                     "Nome = @Nome, " +
                     "NomeUsuario = @NomeUsuario, " +
                     "credencialCartao = @CredencialCartao, " +
                     "credencialTeclado = @CredencialTeclado, " +
                     "senha = @Senha, " +
                     "cargo_idCargo = @CargoId, " +
                     "perfil_idPerfil = @PerfilId " +
                     "WHERE idFuncionario = @IdFuncionario";
            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@Nome", funcionario.Nome);
            command.Parameters.AddWithValue("@NomeUsuario", funcionario.NomeUsuario);
            command.Parameters.AddWithValue("@CredencialCartao", funcionario.CredencialCartao);
            command.Parameters.AddWithValue("@CredencialTeclado", funcionario.CredencialTeclado);
            command.Parameters.AddWithValue("@Senha", funcionario.Senha);
            command.Parameters.AddWithValue("@CargoId", funcionario.Cargo_IdCargo);
            command.Parameters.AddWithValue("@PerfilId", funcionario.Perfil_IdPerfil);

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

}