using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public class TransferDAO : ITransferDAO
    { 

        //public Account GetAccountInfoById(int userId)
        //{
        //    Account account = new Account();

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance from accounts where user_id = @userId", conn);
        //            cmd.Parameters.AddWithValue("@userId", userId);

        //            SqlDataReader reader = cmd.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                account = ConvertReaderToAccount(reader);

        //             
        //        }
        //    }
        //    catch (SqlException)
        //    {
        //        Console.WriteLine("Error getting account info");
        //    }
        //    return account;
        //}
    }
}
