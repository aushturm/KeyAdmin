using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.Model
{
    public class AccountPropertiesItem
    {
        public string identifier;
        public string value;
    }

    public class AccountItem
    {
        public string identifier;
        public AccountPropertiesItem properties;
    }
}
