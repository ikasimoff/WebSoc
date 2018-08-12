using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Lib.IRepository;
using Lib.Models;

namespace Lib.RepositoryDapper
{
    public class UserRepository : IUserRepository
    {

        public static string ConnectionString = @"Server=upax.club,1333;Database=Di;User ID=sa;Password=Ketrine8586";
        private IDbConnection _db = new SqlConnection(ConnectionString);
        public long GetCount()
        {
            var qq = _db.Query<int>("SELECT Count(Id) FROM [User]").First();
            return qq;
        }

        public List<ShortUser> GetLogins(long fromId, long toId)
        {
            List<ShortUser> users = new List<ShortUser>();
            users = _db.Query<ShortUser>(@"
                                SELECT [User].[Login], [User].Id 
                                FROM[User]
                                WHERE
                                [User].Id > " + fromId + @" AND[User].Id < " + toId)
                                .ToList();
            return users;
        }

        public List<ShortUser> GetLoginsNoPost(long fromId, long toId)
        {
            List<ShortUser> users = new List<ShortUser>();
            users = _db.Query<ShortUser>(@"
                                SELECT [User].[Login], [User].Id 
                                FROM[User]
                                WHERE
                                [User].Id > "+fromId+@" AND[User].Id < "+toId+ @"
                                AND[User].Id NOT IN
                                    (SELECT UserId FROM Posts
                                    WHERE UserId > " + fromId + @" and UserId < " + toId + @"
                                    GROUP BY UserId)
                            ").ToList();
            return users;
        }

        //public List<UserAccount>
    }
}
