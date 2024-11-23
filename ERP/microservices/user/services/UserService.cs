using ERP.microservices.user.interfaces;
using ERP.models.user;
using System.Security.Cryptography;
using System.Text;

namespace ERP.microservices.user.services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task RegisterAsync(User user)
        {
            // Hash the password before saving
            user.PasswordHash = HashPassword(user.PasswordHash);
            await _userRepository.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _userRepository.GetByIdAsync(id);

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _userRepository.GetAllAsync();

        public async Task UpdateAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                // Update hashed password if provided
                user.PasswordHash = HashPassword(user.PasswordHash);
            }
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid id) =>
            await _userRepository.DeleteAsync(id);

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}
