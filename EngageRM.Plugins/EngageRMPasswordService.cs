using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngageRM.Plugins
{
    class EngageRMPasswordService
    {
        // ToDo : passwd generator
        public static string GetPassword(string email, string firstname, string lastname)
        {
            return BCrypt.Net.BCrypt.HashPassword(email + firstname + lastname, BCrypt.Net.BCrypt.GenerateSalt(), true);
        }
    }
}
