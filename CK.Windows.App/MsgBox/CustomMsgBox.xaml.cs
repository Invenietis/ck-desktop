﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CK.Windows.App
{
    /// <summary>
    /// Interaction logic for CustomMsgBox.xaml
    /// </summary>
    public partial class CustomMsgBox : Window
    {
        public CustomMsgBox(ref ModalViewModel dataContext)
        {
            dataContext.Holder = this;
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}
