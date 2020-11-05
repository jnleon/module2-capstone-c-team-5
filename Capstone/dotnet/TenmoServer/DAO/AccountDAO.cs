using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountDAO : IAccountDAO
    {
        private readonly string connectionString;

        public AccountDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        //HAS TO BE ADMIN AUTHORIZE
        public List<Account> GetAccountInfo()
        {
            List<Account> accounts = new List<Account>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance from accounts", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Account a = ConvertReaderToAccount(reader);
                        accounts.Add(a);
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return accounts;
        }

        public Account GetAccountInfoById(int userId)
        {
            Account account = new Account();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance from accounts where user_id = @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                   
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                    account = ConvertReaderToAccount(reader);

                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return account;
        }

        private Account ConvertReaderToAccount(SqlDataReader reader)
        {
            Account a = new Account();
            a.AccountId = Convert.ToInt32(reader["account_id"]);
            a.UserId = Convert.ToInt32(reader["user_id"]);
            a.Balance = Convert.ToDecimal(reader["balance"]);

            return a;
        }
    }
}