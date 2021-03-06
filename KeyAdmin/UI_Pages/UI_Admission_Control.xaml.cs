﻿using KeyAdmin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyAdmin.UI_Pages
{
    /// <summary>
    /// Interaction logic for UI_Admission_Control.xaml
    /// </summary>
    public partial class UI_Admission_Control : UserControl, IHavePassword
    {
        public UI_Admission_Control()
        {
            InitializeComponent();
        }

        public System.Security.SecureString Password
        {
            get
            {
                return PWBox.SecurePassword;
            }
        }
    }
}
