using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferDAO transferDAO;
        private readonly IAccountDAO accountDAO;

        public TransferController(ITransferDAO _transferDAO, IAccountDAO _accountDAO)
        {
            transferDAO = _transferDAO;
            accountDAO = _accountDAO;
        }

        [HttpPost]
        public decimal TransferFunds(Transfer transfer)
        {
            Account sender = accountDAO.GetAccountInfoById(transfer.UserFromId);
            Account receiver = accountDAO.GetAccountInfoById(transfer.UserToId);

            return transferDAO.MakeTransfer(transfer.Amount, sender, receiver);
        }

        [HttpGet]
        public List<Transfer> GetTransfers()
        {
            return transferDAO.GetPastTransfers((int)GetCurrentUserId());
        }
        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }
    }
}