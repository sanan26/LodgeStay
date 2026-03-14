using System;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class SessionService
    {
        public User? CurrentUser { get; private set; }
        public bool IsloggedIn => CurrentUser != null;
        public bool IsStaff => CurrentUser?.Role == "Staff";
        public bool IsManager => CurrentUser?.Role == "Manager";
        public bool IsOwner => CurrentUser?.Role == "Owner";
        public void Login(User user)
        {
            CurrentUser = user;
        }
        public void Logout()
        {
            CurrentUser = null;
        }

    }
}
