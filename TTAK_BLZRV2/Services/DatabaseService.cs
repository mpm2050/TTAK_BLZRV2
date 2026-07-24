using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using TTAK_BLZRV2.Models;

namespace TTAK_BLZRV2.Services
{
 

    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        private async Task Init()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(
                FileSystem.AppDataDirectory,
                "app.db3");

            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<Customer>();
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            await Init();

            return await _database!
                .Table<Customer>()
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerAsync(int id)
        {
            await Init();

            return await _database!
                .Table<Customer>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveCustomerAsync(Customer customer)
        {
            await Init();

            if (customer.Id != 0)
                return await _database!.UpdateAsync(customer);

            return await _database!.InsertAsync(customer);
        }

        public async Task<int> DeleteCustomerAsync(Customer customer)
        {
            await Init();

            return await _database!.DeleteAsync(customer);
        }
    }
}
