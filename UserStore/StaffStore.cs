using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace YMCAProject.UserStore;

public class StaffStore : IUserStore<Models.Staff>, IUserPasswordStore<Models.Staff>
{
    private readonly string _connectionString;

    public StaffStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IdentityResult> CreateAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("INSERT INTO Staff (fname, lname, email, PasswordHash, is_active, is_admin) VALUES (@Fname, @Lname, @email, @PasswordHash, @is_active, @is_admin)", connection);
            command.Parameters.AddWithValue("@Fname", user.fname);
            command.Parameters.AddWithValue("@Lname", user.lname);
            command.Parameters.AddWithValue("@Email", user.email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@IsActive", user.is_active);
            command.Parameters.AddWithValue("@IsAdmin", user.is_admin);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("UPDATE Staff SET fname = @FirstName, lname = @LastName, email = @Email, PasswordHash = @PasswordHash, is_active = @IsActive, is_admin = @IsAdmin WHERE staff_id = @Id", connection);
            command.Parameters.AddWithValue("@Id", user.staff_id);
            command.Parameters.AddWithValue("@FirstName", user.fname);
            command.Parameters.AddWithValue("@LastName", user.lname);
            command.Parameters.AddWithValue("@Email", user.email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@IsActive", user.is_active);
            command.Parameters.AddWithValue("@IsAdmin", user.is_admin);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("DELETE FROM Staff WHERE staff_d = @Id", connection);
            command.Parameters.AddWithValue("@Id", user.staff_id);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<Models.Staff?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        Models.Staff? staff = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("SELECT * FROM Staff WHERE staff_id = @Id", connection);
            command.Parameters.AddWithValue("@Id", userId);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    staff = new Models.Staff
                    {
                        staff_id = reader.GetInt32(reader.GetOrdinal("staff_id")),
                        fname = reader.GetString(reader.GetOrdinal("fname")),
                        lname = reader.GetString(reader.GetOrdinal("lname")),
                        email = reader.GetString(reader.GetOrdinal("email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        is_active = reader.GetBoolean(reader.GetOrdinal("is_active")),
                        is_admin = reader.GetBoolean(reader.GetOrdinal("is_admin"))
                    };
                }
            }
        }
        return staff;
    }

    public async Task<Models.Staff?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        Models.Staff? staff = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("SELECT * FROM Staff WHERE email = @Email", connection);
            command.Parameters.AddWithValue("@Email", normalizedUserName);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    staff = new Models.Staff
                    {
                        staff_id = reader.GetInt32(reader.GetOrdinal("staff_id")),
                        fname = reader.GetString(reader.GetOrdinal("fname")),
                        lname = reader.GetString(reader.GetOrdinal("lname")),
                        email = reader.GetString(reader.GetOrdinal("email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        is_active = reader.GetBoolean(reader.GetOrdinal("is_active")),
                        is_admin = reader.GetBoolean(reader.GetOrdinal("is_admin"))
                    };
                }
            }
        }
        return staff;
    }

    public Task<string> GetUserIdAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.staff_id.ToString());
    }

    public Task<string> GetUserNameAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.email);
    }

    public Task SetUserNameAsync(Models.Staff user, string userName, CancellationToken cancellationToken)
    {
        user.email = userName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.email.ToUpperInvariant());
    }

    public Task SetNormalizedUserNameAsync(Models.Staff user, string normalizedName, CancellationToken cancellationToken)
    {
        // Normalization logic could be implemented here, if needed.
        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(Models.Staff user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    }

    public Task SetPasswordHashAsync(Models.Staff user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // Dispose of any resources if necessary
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
