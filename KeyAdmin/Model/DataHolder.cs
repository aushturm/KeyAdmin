using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.Model
{
    public class AccountPropertiesItem
    {
        public string Identifier { get; set; }
        public string Value { get; set; }
    }

    public class AccountItem
    {
        public string Identifier { get; set; }
        public List<AccountPropertiesItem> Properties { get; set; }
    }
}
