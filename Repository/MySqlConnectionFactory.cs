namespace ProjetoFechadura.Repository;
using MySql.Data.MySqlClient;


public class MySqlConnectionFactory
{
    public static MySqlConnection GetConnection()
    {
        const string connectionString = "server=localhost;" +
                                        "port=3306;" +
                                        "database=bdfechadura;" +
                                        "uid=root;" +
                                        "pwd=senai2024;";
        return new MySqlConnection(connectionString);
    }
}