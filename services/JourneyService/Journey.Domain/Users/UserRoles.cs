using Journey.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Domain.Users
{
    public static class UserRoles
    {
        public static readonly Error NotFound = Error.NotFound(
            "UserRoles.NotFound", "The specified role does not exist!");

        public static readonly string Admin = "Admin";
        public static readonly string User = "User";


        public static Result<string> GetRole(string value)
        {
            return value.ToLower() switch
            {
                "admin" => Admin,
                "user" => User,
                _ => Result.Failure<string>(NotFound)
            };

        }
    }
}
