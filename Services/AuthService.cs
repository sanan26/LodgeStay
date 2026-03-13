using System;
using LodgeStay.Data;
using LodgeStay.Models;
using BCrypt.Net;

namespace LodgeStay.Services
{
    public class AuthService
    {
        private readonly DatabaseContext _database;

        public AuthService(DatabaseContext database)
        {
            _database = database;
        }
        
        public async Task<bool> SignupAsync(User user)
        {
            var existingUser = await _database.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
               return false; // User already exists
            }
            else
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                await _database.InsertUserAsync(user);
                return true; // Signup successful
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _database.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null; // User not found
            }
            else
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (isPasswordValid && user.IsVerified)
                {
                    return user; // Login successful
                }
                else
                {
                    return null; // Invalid password
                }
            }
        }
    }
}
