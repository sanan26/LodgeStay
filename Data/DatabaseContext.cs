using System;
using SQLite;
using LodgeStay.Models;
using System.Threading.Tasks;

namespace LodgeStay.Data
{
    public class DatabaseContext
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseContext(string dbpath) 
        {
            _database = new SQLiteAsyncConnection(dbpath);
        }

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Room>();
            await _database.CreateTableAsync<Reservation>();
        }
    }
}
