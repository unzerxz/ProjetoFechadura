using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Models;
using ProjetoFechadura.Repository;

public class PerfilDAO
{
    private readonly MySqlConnection _connection;

    public PerfilDAO()
    {
        _connection = MySqlConnectionFactory.GetConnection();
    }

    private static List<Perfil?> ReadAll(MySqlCommand command)
    {
        var perfis = new List<Perfil?>();

        using var reader = command.ExecuteReader();
        if (!reader.HasRows) return perfis;
        while (reader.Read())
        {
            var perfil = new Perfil
            {
                IdPerfil = reader.GetInt32("idPerfil"),
                TipoPerfil = reader.GetInt32("tipoPerfil")
            };
            perfis.Add(perfil);
        }

        return perfis;
    }

    public List<Perfil?> Read()
    {
        List<Perfil?> perfis = null!;

        try
        {
            _connection.Open();
            const string query = "SELECT * FROM perfil";

            var command = new MySqlCommand(query, _connection);

            perfis = ReadAll(command);
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

        return perfis;
    }

    public Perfil? ReadById(int id)
    {
        Perfil? perfil = null!;

        try
        {
            _connection.Open();
            var query = "SELECT * FROM bdfechadura.perfil WHERE idPerfil = @Id";

            var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            perfil = ReadAll(command).FirstOrDefault();
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

        return perfil;
    }

    public int Create(Perfil perfil)
    {
        int id = 0;
        try
        {
            _connection.Open();
            const string query = "INSERT INTO perfil (tipoPerfil) " +
                                 "VALUES (@TipoPerfil)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@TipoPerfil", perfil.TipoPerfil);

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

    public void Update(int id, Perfil perfil)
    {
        try
        {
            _connection.Open();
            const string query = "UPDATE perfil SET " +
                                 "tipoPerfil = @TipoPerfil " +
                                 "WHERE idPerfil = @IdPerfil";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@TipoPerfil", perfil.TipoPerfil);
            command.Parameters.AddWithValue("@IdPerfil", id);

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

    public void Delete(int idPerfil)
    {
        try
        {
            _connection.Open();
            const string query = "DELETE FROM perfil WHERE idPerfil = @IdPerfil";

            using var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@IdPerfil", idPerfil);
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
