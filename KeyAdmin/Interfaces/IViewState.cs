using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.Interfaces
{
    public enum ViewState { Running, Finished, Error}

    public interface IUIPages
    {
        event EventHandler<EventArgs.ViewStateChangedEventArgs> ViewStateChanged;
        object[] Parameters { set; }
    }
}
