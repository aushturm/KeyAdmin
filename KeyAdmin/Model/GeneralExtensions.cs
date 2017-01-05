using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyAdmin.Model
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// Shows a error message to the user
        /// </summary>
        /// <param name="errorMsg">The Error Message to be shown</param>
        public static void ShowErrorMessage(this string errorMsg)
        {
            MessageBox.Show(errorMsg, "An Error Occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// Shows a information message to the user
        /// </summary>
        /// <param name="message">The message to be shown</param>
        public static void ShowInfoMessage(this string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
