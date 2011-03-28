using System;
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
using System.Windows.Shapes;
using SqlScriptPackager.Core;

namespace SqlScriptPackager.WPF
{
    /// <summary>
    /// Interaction logic for ViewConnectionInfo.xaml
    /// </summary>
    public partial class ViewConnectionInfo : Window
    {
        public IEnumerable<DatabaseConnection> Connections
        {
            get;
            protected set;
        }

        public ViewConnectionInfo(IEnumerable<DatabaseConnection> connections)
        {
            this.Connections = connections;
            InitializeComponent();
        }
    }
}
