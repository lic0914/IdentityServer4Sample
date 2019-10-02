using System.Collections.Generic;

namespace AuthServer
{
    public class Identity
    {
        private static Dictionary<string,string> _account=new Dictionary<string, string>
        {
            {"admin","123" },
            {"lic","123" },
        };

        public static bool IsLogin { get;private set; }


        public static bool Login(string username, string pwd)
        {
            if (_account.ContainsKey(username) && _account[username] == pwd)
            {
                IsLogin = true;
                return true;

            }
            return false;
        }
    }

}