using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Models;

namespace Lib.IRepository
{
    interface IUserRepository
    {
        long GetCount();
        IList<ShortUser> GetLogins(long fromId, long toId);
        IList<ShortUser> GetLoginsNoPost(long fromId, long toId);
    }
}
