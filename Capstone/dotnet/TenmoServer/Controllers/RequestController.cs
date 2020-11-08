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
    public class RequestController : Controller
    {
        private readonly IRequestDAO requestDAO;
        private readonly IAccountDAO accountDAO;

        public RequestController(IRequestDAO _requestDAO, IAccountDAO _accountDAO)
        {
            requestDAO = _requestDAO;
            accountDAO = _accountDAO;
        }

        [HttpGet]
        public List<Transfer> GetTransfers()
        {
            return requestDAO.GetTransferRequests((int)GetCurrentUserId());
        }
        [HttpGet("{transferId}")]
        public Transfer GetTransferById(int transferId)
        {
            return requestDAO.GetTransferById((int)GetCurrentUserId(), transferId);
        }
        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }

        [HttpPost]
        public ActionResult<decimal> RequestTransfer(Transfer transfer)
        {
            Account someone = accountDAO.GetAccountInfoById(transfer.UserFromId);
            Account me = accountDAO.GetAccountInfoById(transfer.UserToId);

            if (someone.AccountId == 0)
            {
                return NotFound();
            }

            return requestDAO.RequestTransfer(transfer.Amount, someone, me);
        }

        [HttpPut("{transferId}")]
        public ActionResult<decimal> RejectTransfer(int transferId, Transfer transfer)
        {
            return requestDAO.RejectTransfer(transferId, transfer);
        }

        [HttpPut("{transferId}")]
        public ActionResult<decimal> AcceptTransfer(int transferId, Transfer transfer)//I added "{transferId}" & "int transferId" & made it a parameter in the AcceptTransferRequest
        {
            Account sender = accountDAO.GetAccountInfoById(transfer.UserFromId);
            Account receiver = accountDAO.GetAccountInfoById(transfer.UserToId);

            if (receiver.AccountId == 0)
            {
                return NotFound();
            }

            return requestDAO.AcceptTransferRequest(sender, receiver, transfer, transferId);
        }
    }
}
