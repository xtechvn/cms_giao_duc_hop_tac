using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.IRepositories
{
    public interface IUserAgentRepository
    {
        UserAgent GetUserAgentClient(int ClientId);
    }
}
