using Microsoft.Data.SqlClient;

namespace Films.DAL
{
    public class DBConnection
    {
        private SqlConnection connection;
        public SqlConnection Connection { get { return connection; } }

        public DBConnection()
        {
            CreateConnection();
        }

        private void CreateConnection()
        {
            try
            {
                string connectionString = "Data Source=46.183.117.154,54321;" +
                    "Initial Catalog=0Films;" +
                    "User ID=sa;" +
                    "Password=Sql#123456789;" +
                    "TrustServerCertificate=True;";
                connection = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la conexión a la base de datos", ex);
            }
        }

        public void OpenConnection()
        {
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
