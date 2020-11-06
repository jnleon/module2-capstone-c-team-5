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
    public class UsersController : Controller
    {
        private readonly IUserDAO userDAO;

        public UsersController(IUserDAO _userDAO)
        {
            userDAO = _userDAO;
        }

        [HttpGet("all")]
        public List<User> GetUserList()
        {
            return userDAO.GetUsers();
        }
    }
}