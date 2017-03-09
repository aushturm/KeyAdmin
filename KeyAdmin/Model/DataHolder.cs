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

        public AccountItem()
        {
            Properties = new List<AccountPropertiesItem>();
        }

        public AccountItem Clone()
        {
            AccountItem clone = new AccountItem();
            clone.Identifier = Identifier;
            foreach (AccountPropertiesItem propertie in Properties)
            {
                AccountPropertiesItem clonePropertie = new AccountPropertiesItem();
                clonePropertie.Identifier = propertie.Identifier;
                clonePropertie.Value = propertie.Value;
                clone.Properties.Add(clonePropertie);
            }
            return clone;
        }
    }
}
