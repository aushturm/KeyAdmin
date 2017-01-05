using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.Interfaces
{
    public interface IHavePassword
    {
        System.Security.SecureString Password { get; }
    }

    public interface IHavePasswordsToChange
    {
        System.Security.SecureString OldPassword { get; }
        System.Security.SecureString NewPassword { get; }
        System.Security.SecureString ConfirmPassword { get; }
    }
}
