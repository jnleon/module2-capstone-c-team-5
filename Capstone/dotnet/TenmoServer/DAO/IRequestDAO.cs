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
        decimal RejectTransfer(int transferId, Transfer transfer);
        decimal AcceptTransferRequest(Account sender, Account receiver, Transfer transfer, int transferId);
        List<Transfer> GetTransferRequests(int userId);
        Transfer GetTransferById(int userId, int transferId);
    }
}