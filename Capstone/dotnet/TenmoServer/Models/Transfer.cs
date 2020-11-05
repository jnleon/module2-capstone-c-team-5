using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int UserFromId { get; set; }
        public int UserToId { get; set; }
        public decimal Amount { get; set; }
        public string UserNameFrom { get; set; }
        public string UserNameTo { get; set; }
    }
}
