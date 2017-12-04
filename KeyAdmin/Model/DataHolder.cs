using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeyAdmin.Model
{
    [XmlRoot("DeserializeItemHolder")]
    public class DeserializeItemHolder
    {
        [XmlElement("AccountItem")]
        public List<AccountItem> Data { get; set; }
    }

    public class AccountPropertiesItem : ICloneable
    {
        [XmlElement("Identifier")]
        public string Identifier { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }


    public class AccountItem : ICloneable
    {
        [XmlElement("Identifier")]
        public string Identifier { get; set; }

        [XmlElement("AccountPropertiesItem")]
        public List<AccountPropertiesItem> Properties { get; set; }

        public AccountItem()
        {
            Properties = new List<AccountPropertiesItem>();
        }

        public object Clone()
        {
            //AccountItem clone = new AccountItem();
            //clone.Identifier = Identifier;
            //foreach (AccountPropertiesItem propertie in Properties)
            //{
            //    AccountPropertiesItem clonePropertie = new AccountPropertiesItem();
            //    clonePropertie.Identifier = propertie.Identifier;
            //    clonePropertie.Value = propertie.Value;
            //    clone.Properties.Add(clonePropertie);
            //}
            //return clone;
            return MemberwiseClone();
        }
    }
}
