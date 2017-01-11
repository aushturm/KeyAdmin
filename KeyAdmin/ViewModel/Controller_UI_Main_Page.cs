using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyAdmin.Interfaces;
using KeyAdmin.EventArgs;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages
    {
        #region events
        public event EventHandler<ViewStateChangedEventArgs> ViewStateChanged;
        #endregion

        #region raise event handler
        protected void OnViewStateChanged(ViewStateChangedEventArgs e)
        {
            EventHandler<ViewStateChangedEventArgs> handler = ViewStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
