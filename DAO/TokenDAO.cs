using System;
using MySql.Data.MySqlClient;
using ProjetoFechadura.Repository;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.DAO
{
    public class TokenDAO
    {
        private readonly MySqlConnection _connection;

        public TokenDAO()
        {
            _connection = MySqlConnectionFactory.GetConnection();
        }

        public void SaveToken(string token, DateTime expirationTime, int funcionarioId)
        {
            try
            {
                _connection.Open();
                const string query = @"INSERT INTO tokenFuncionario (token, timeExpiracao, funcionario_idFuncionario) 
                                       VALUES (@Token, @ExpirationTime, @FuncionarioId)";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@ExpirationTime", expirationTime);
                command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar token: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool IsValidTokenExists(int funcionarioId)
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT COUNT(*) FROM tokenFuncionario 
                                       WHERE funcionario_idFuncionario = @FuncionarioId 
                                       AND timeExpiracao > @CurrentTime";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar token válido: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void RemoveToken(string token)
        {
            try
            {
                _connection.Open();
                const string query = "DELETE FROM tokenFuncionario WHERE token = @Token";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Token", token);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover token: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void RemoveExpiredTokens()
        {
            try
            {
                _connection.Open();
                const string query = "DELETE FROM tokenFuncionario WHERE timeExpiracao <= @CurrentTime";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover tokens expirados: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT COUNT(*) FROM tokenFuncionario 
                                       WHERE token = @Token 
                                       AND timeExpiracao > @CurrentTime";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar validade do token: {ex.Message}");
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool IsTokenValidAndActive(string token)
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT COUNT(*) FROM tokenFuncionario 
                                       WHERE token = @Token 
                                       AND timeExpiracao > @CurrentTime";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar validade do token: {ex.Message}");
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public TokenFuncionario GetValidTokenForFuncionario(int funcionarioId)
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT token, timeExpiracao FROM tokenFuncionario 
                                       WHERE funcionario_idFuncionario = @FuncionarioId 
                                       AND timeExpiracao > @CurrentTime
                                       LIMIT 1";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new TokenFuncionario
                    {
                        Token = reader.GetString("token"),
                        TimeExpiracao = reader.GetDateTime("timeExpiracao")
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar token válido: {ex.Message}");
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<TokenFuncionario> ObterTokensExpirados()
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT idToken, token FROM tokenFuncionario 
                                       WHERE timeExpiracao <= @CurrentTime";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                using var reader = command.ExecuteReader();
                var tokensExpirados = new List<TokenFuncionario>();

                while (reader.Read())
                {
                    tokensExpirados.Add(new TokenFuncionario
                    {
                        IdToken = reader.GetInt32("idToken"),
                        Token = reader.GetString("token")
                    });
                }

                return tokensExpirados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar tokens expirados: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void RemoverToken(int idToken)
        {
            try
            {
                _connection.Open();
                const string query = "DELETE FROM tokenFuncionario WHERE idToken = @IdToken";

                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@IdToken", idToken);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover token: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void LogAllTokens()
        {
            try
            {
                _connection.Open();
                const string query = @"SELECT idToken, token, timeExpiracao FROM tokenFuncionario";

                using var command = new MySqlCommand(query, _connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idToken = reader.GetInt32("idToken");
                    string token = reader.GetString("token");
                    DateTime expiracao = reader.GetDateTime("timeExpiracao");
                    Console.WriteLine($"Token ID: {idToken}, Token: {token}, Expira em: {expiracao}, Expirado: {expiracao <= DateTime.UtcNow}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao logar tokens: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
