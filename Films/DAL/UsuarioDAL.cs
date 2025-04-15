using Films.Models;
using Microsoft.Data.SqlClient;
namespace Films.DAL
{
    public class UsuarioDAL
    {
        private DBConnection dbConn;
        public UsuarioDAL()
        {
            dbConn = new DBConnection();
        }

        public Usuario GetUsuarioLogin(string userName, string pwd)
        {
            try
            {
                dbConn.OpenConnection();
                string sql = $"SELECT * FROM Usuario WHERE UserName = @UserName";

                SqlCommand cmd = new SqlCommand(sql, dbConn.Connection);
                cmd.Parameters.AddWithValue("@UserName", userName);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var passwordHash = (byte[])reader["PasswordHash"];
                    var passwordSalt = (byte[])reader["PasswordSalt"];

                    if (PasswordHelper.VerifyPasswordHash(pwd, passwordHash, passwordSalt))
                    {
                        return new Usuario
                        {
                            IdUsuario = (int)reader["IdUsuario"],
                            UserName = (string)reader["UserName"],
                            PasswordHash = (byte[])reader["PasswordHash"],
                            PasswordSalt = (byte[])reader["PasswordSalt"],
                            Apellido = (string)reader["Apellido"],
                            Email = reader["Email"] as string,
                            FechaNacimiento = reader["FechaNacimiento"] as DateTime?,
                            Telefono = reader["Telefono"] as string,
                            Direccion = reader["Direccion"] as string,
                            Ciudad = reader["Ciudad"] as string,
                            Estado = reader["Estado"] as string,
                            CodigoPostal = reader["CodigoPostal"] as string,
                            FechaRegistro = (DateTime)reader["FechaRegistro"],
                            Activo = (bool)reader["Activo"]
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
        public void CreateUsuario(Usuario usuario, string password)
        {
            if (usuario != null)
            {
                try
                {
                    PasswordHelper.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                    dbConn.OpenConnection();

                    string sql = "INSERT INTO Usuario (UserName, PasswordHash, PasswordSalt, Apellido, Email, FechaNacimiento, Telefono, Direccion, Ciudad, Estado, CodigoPostal, FechaRegistro, Activo) VALUES(@UserName, @PasswordHash, @PasswordSalt, @Apellido, @Email, @FechaNacimiento, @Telefono, @Direccion, @Ciudad, @Estado, @CodigoPostal, @FechaRegistro, @Activo);";

                    using (SqlCommand cmd = new SqlCommand(sql, dbConn.Connection))
                    {
                        cmd.Parameters.AddWithValue("@UserName", usuario.UserName);

                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);

                        cmd.Parameters.AddWithValue("@Apellido", "ejemplo");

                        cmd.Parameters.AddWithValue("@Email", "ejemplo");

                        cmd.Parameters.AddWithValue("@FechaNacimiento", "1990-01-01");

                        cmd.Parameters.AddWithValue("@Telefono", "123456789");

                        cmd.Parameters.AddWithValue("@Direccion", "ejemplo");

                        cmd.Parameters.AddWithValue("@Ciudad", "ejemplo");

                        cmd.Parameters.AddWithValue("@Estado", "ejemplo");

                        cmd.Parameters.AddWithValue("@CodigoPostal", "123");

                        cmd.Parameters.AddWithValue("@FechaRegistro", "1990-01-01");

                        cmd.Parameters.AddWithValue("@Activo", true);

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
