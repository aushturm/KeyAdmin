using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.EventArgs
{
    public class ViewStateChangedEventArgs : System.EventArgs
    {
        public Interfaces.ViewState viewState;

        public string Message;
    }
}
