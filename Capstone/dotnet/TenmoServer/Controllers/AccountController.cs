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
    public class AccountController : Controller
    {
        private readonly IAccountDAO accountDAO;

        public AccountController(IAccountDAO _accountDAO)
        {
            accountDAO = _accountDAO;
        }
       
        //ADMINNNN
        //dont need :)
        [HttpGet("all")]
        public List<Account> GetAccounts()
        {
            return accountDAO.GetAccountInfo();
        }


    
        [HttpGet("")]
        public Account GetMyAccountBalance()
        {
            return accountDAO.GetAccountInfoById((int)GetCurrentUserId());
        }
        private  int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }

    }
}
