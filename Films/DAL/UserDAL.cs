using Films.Models;
using Microsoft.Data.SqlClient;
namespace Films.DAL
{
    public class UserDAL
    {
        private DBConnection dbConn;
        public UserDAL()
        {
            dbConn = new DBConnection();
        }

        public User GetUsuarioLogin(string username, string pwd)
        {
            try
            {
                dbConn.OpenConnection();
                string sql = $"SELECT * FROM User WHERE Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, dbConn.Connection);
                cmd.Parameters.AddWithValue("@Username", username);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var passwordHash = (byte[])reader["PasswordHash"];
                    var passwordSalt = (byte[])reader["PasswordSalt"];

                    if (PasswordHelper.VerifyPasswordHash(pwd, passwordHash, passwordSalt))
                    {
                        return new User
                        {
                            IdUsuario = (int)reader["IdUsuario"],
                            Username = (string)reader["Username"],
                            PasswordHash = (byte[])reader["PasswordHash"],
                            PasswordSalt = (byte[])reader["PasswordSalt"],
                            Email = (string)reader["Email"],
                            Image = (string)reader["Image"],
                            AboutMe = reader["AboutMe"] != DBNull.Value ? (string)reader["AboutMe"] : null
                        };
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dbConn.CloseConnection();
            }
            return null;
        }
        public void CreateUsuario(User usuario, string password)
        {
            if (usuario != null)
            {
                try
                {
                    PasswordHelper.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                    dbConn.OpenConnection();

                    string sql = "INSERT INTO User (Username, PasswordHash, PasswordSalt, Email, AboutMe, Image) VALUES(@Username, @PasswordHash, @PasswordSalt, @Email, @AboutMe, @Image);";

                    using (SqlCommand cmd = new SqlCommand(sql, dbConn.Connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", usuario.Username);

                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);

                        cmd.Parameters.AddWithValue("@Email", "ejemplo");

                        cmd.Parameters.AddWithValue("@AboutMe", "ejemplo");

                        cmd.Parameters.AddWithValue("@Image", "ejemplo.png");

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    dbConn.CloseConnection();
                }
            }
        }
    }
}
