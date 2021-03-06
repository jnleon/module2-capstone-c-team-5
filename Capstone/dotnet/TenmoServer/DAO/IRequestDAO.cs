﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IRequestDAO
    {
        decimal RequestTransfer(decimal amountToTransfer, Account someone, Account me);
        Transfer RejectTransfer(Transfer transfer);
        Transfer AcceptTransferRequest(Account sender, Account receiver, Transfer transfer);
        List<Transfer> GetTransferRequests(int userId);
        Transfer GetTransferById(int userId, int transferId);
    }
}
