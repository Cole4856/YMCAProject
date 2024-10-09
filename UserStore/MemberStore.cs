using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using YMCAProject.Models;
#nullable enable


namespace YMCAProject.UserStore;

public class MemberStore: IUserStore<Member>, IUserPasswordStore<Models.Member>
{
    private readonly string _connectionString;

    public MemberStore(string connectionString)
    {
        _connectionString = connectionString;
    }

     public async Task<IdentityResult> CreateAsync(Models.Member user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("INSERT INTO Members (FirstName, LastName, Email, PasswordHash, IsActive, IsMember) VALUES (@FirstName, @LastName, @Email, @PasswordHash, @IsActive, @IsMember)", connection);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@IsMember", user.IsMember);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Models.Member user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("UPDATE Members SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PasswordHash = @PasswordHash, IsActive = @IsActive, IsMember = @IsMember WHERE MemberId = @MemberId", connection);
            command.Parameters.AddWithValue("@MemberId", user.MemberId);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@IsMember", user.IsMember);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Models.Member user, CancellationToken cancellationToken)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("DELETE FROM Members WHERE MemberId = @MemberId", connection);
            command.Parameters.AddWithValue("@MemberId", user.MemberId);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        return IdentityResult.Success;
    }

    public async Task<Member?> FindByIdAsync(string userId, CancellationToken cancellationToken)
{
    // Declare member as nullable
    Models.Member? member = null;

    // Check for null or empty userId
    if (string.IsNullOrWhiteSpace(userId))
    {
        throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
    }

    using (var connection = new MySqlConnection(_connectionString))
    {
        await connection.OpenAsync(cancellationToken);
        var command = new MySqlCommand("SELECT * FROM Members WHERE MemberId = @MemberId", connection);
        command.Parameters.AddWithValue("@MemberId", userId);
        
        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                member = new Models.Member
                {
                    MemberId = reader.GetInt32(reader.GetOrdinal("MemberId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    IsMember = reader.GetBoolean(reader.GetOrdinal("IsMember"))
                };
            }
        }
    }

    // Return the member, which can be null if not found
    return member; 
}


    public async Task<Models.Member?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        Models.Member? member = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var command = new MySqlCommand("SELECT * FROM Members WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", normalizedUserName);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    member = new Models.Member
                    {
                        MemberId = reader.GetInt32(reader.GetOrdinal("MemberId")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        IsMember = reader.GetBoolean(reader.GetOrdinal("IsMember"))
                    };
                }
            }
        }
        return member;
    }

    public Task<string> GetUserIdAsync(Models.Member user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.MemberId.ToString());
    }

    public Task<string> GetUserNameAsync(Models.Member user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task SetUserNameAsync(Models.Member user, string userName, CancellationToken cancellationToken)
    {
        user.Email = userName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(Models.Member user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email.ToUpperInvariant());
    }

    public Task SetNormalizedUserNameAsync(Models.Member user, string normalizedName, CancellationToken cancellationToken)
    {
        // Normalization logic could be implemented here, if needed.
        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(Models.Member user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(Models.Member user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    }

    public Task SetPasswordHashAsync(Models.Member user, string passwordHash, CancellationToken cancellationToken)
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
