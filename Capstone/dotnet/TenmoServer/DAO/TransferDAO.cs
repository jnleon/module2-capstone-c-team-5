using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        //method to transfer money between users... reduce the sender's balance and increase the receiver's balance... return sender balance
        public decimal MakeTransfer(decimal amountToTransfer, Account sender, Account receiver)
        {
            if(amountToTransfer <= sender.Balance)
            {
                sender.Balance -= amountToTransfer;
                receiver.Balance += amountToTransfer;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = @senderBalance WHERE user_id = @senderId; " +
                                                    "UPDATE accounts SET balance = @receiverBalance WHERE user_id = @receiverId; " +
                                                    "INSERT transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                                    "VALUES (2, 2, @accountFrom, @accountTo, @amount);", conn);

                    cmd.Parameters.AddWithValue("@senderBalance", sender.Balance);
                    cmd.Parameters.AddWithValue("@receiverBalance", receiver.Balance);
                    cmd.Parameters.AddWithValue("@senderId", sender.UserId);
                    cmd.Parameters.AddWithValue("@receiverId", receiver.UserId);

                    cmd.Parameters.AddWithValue("@accountFrom", sender.AccountId);
                    cmd.Parameters.AddWithValue("@accountTo", receiver.AccountId);
                    cmd.Parameters.AddWithValue("@amount", amountToTransfer);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }            
            return sender.Balance;
        }

        public List<Transfer> GetPastTransfers(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS 'UserNameFrom', ub.username as 'UserNameTo' " +
                                                    "FROM transfers t " +
                                                    "INNER JOIN accounts a ON t.account_from = a.account_id " +
                                                    "INNER JOIN users u ON a.user_id = u.user_id " +
                                                    "INNER JOIN accounts ab ON t.account_to = ab.account_id " +
                                                    "INNER JOIN users ub ON ab.user_id = ub.user_id " +
                                                    "WHERE u.user_id = @userId OR ub.user_id = @userId;", conn);

                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Transfer t = ConvertReaderToTransfer(reader);
                        transferList.Add(t);
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return transferList;
        }

        private Transfer ConvertReaderToTransfer(SqlDataReader reader)
        {
            Transfer t = new Transfer();
            t.TransferId = Convert.ToInt32(reader["transfer_id"]);
            t.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            t.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            t.UserFromId = Convert.ToInt32(reader["account_from"]);
            t.UserToId = Convert.ToInt32(reader["account_to"]);
            t.Amount = Convert.ToDecimal(reader["amount"]);
            t.UserNameFrom = Convert.ToString(reader["UserNameFrom"]);
            t.UserNameTo = Convert.ToString(reader["UserNameTo"]);

            return t;
        }
    }
}
