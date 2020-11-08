using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class RequestDAO : IRequestDAO
    {
        private readonly string connectionString;

        public RequestDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public decimal RequestTransfer(decimal amountToTransfer, Account someone, Account me)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                                    "VALUES (1, 1, @accountFrom, @accountTo, @amount);", conn);

                    cmd.Parameters.AddWithValue("@accountFrom", someone.AccountId);
                    cmd.Parameters.AddWithValue("@accountTo", me.AccountId);
                    cmd.Parameters.AddWithValue("@amount", amountToTransfer);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return me.Balance;
        }

        public Transfer RejectTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE transfers " +
                                                    "SET transfer_status_id = 3 " +
                                                    "WHERE transfer_id = @transfer_id;", conn);

                    cmd.Parameters.AddWithValue("@transfer_id", transfer.TransferId);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return null;
        }

        public Transfer AcceptTransferRequest(Account sender, Account receiver, Transfer transfer)
        {
            if (receiver.AccountId != 0 && transfer.Amount <= sender.Balance)
            {

                sender.Balance -= transfer.Amount;
                receiver.Balance += transfer.Amount;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = @senderBalance WHERE user_id = @senderId; " +
                                                        "UPDATE accounts SET balance = @receiverBalance WHERE user_id = @receiverId; " +
                                                        "UPDATE transfers SET transfer_status_id = 2 WHERE transfer_id = @transfer_id;", conn);

                        cmd.Parameters.AddWithValue("@senderBalance", sender.Balance);
                        cmd.Parameters.AddWithValue("@receiverBalance", receiver.Balance);
                        cmd.Parameters.AddWithValue("@senderId", sender.UserId);
                        cmd.Parameters.AddWithValue("@receiverId", receiver.UserId);

                        cmd.Parameters.AddWithValue("@transfer_id", transfer.TransferId);
                        cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (SqlException)
                {
                    Console.WriteLine("Error getting account info");
                }
            }
            else
            {
                Console.WriteLine("Invalid Input");
            }
            return null;
        }

        public List<Transfer> GetTransferRequests(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS 'UserNameFrom', ub.username AS 'UserNameTo' " +
                                                    "FROM transfers t " +
                                                    "INNER JOIN accounts a ON t.account_from = a.account_id " +
                                                    "INNER JOIN users u ON a.user_id = u.user_id " +
                                                    "INNER JOIN accounts ab ON t.account_to = ab.account_id " +
                                                    "INNER JOIN users ub ON ab.user_id = ub.user_id " +
                                                    "WHERE t.transfer_status_id = 1 AND t.transfer_type_id = 1 AND t.account_from = @userID;", conn);

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

        public Transfer GetTransferById(int userId, int transferId)
        {
            Transfer t = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS 'UserNameFrom', ub.username AS 'UserNameTo' " +
                                                    "FROM transfers t " +
                                                    "INNER JOIN accounts a ON t.account_from = a.account_id " +
                                                    "INNER JOIN users u ON a.user_id = u.user_id " +
                                                    "INNER JOIN accounts ab ON t.account_to = ab.account_id " +
                                                    "INNER JOIN users ub ON ab.user_id = ub.user_id " +
                                                    "WHERE t.transfer_status_id = 1 AND t.transfer_type_id = 1 AND t.account_from = @userID AND t.transfer_id = @transfer_id;", conn);

                    cmd.Parameters.AddWithValue("@transfer_id", transferId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        t = ConvertReaderToTransfer(reader);
                    }
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Error getting account info");
            }
            return t;
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
