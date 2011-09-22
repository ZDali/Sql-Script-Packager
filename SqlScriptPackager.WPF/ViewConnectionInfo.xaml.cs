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
            this.ContentRendered += new EventHandler(ViewConnectionInfo_ContentRendered);
        }

        void ViewConnectionInfo_ContentRendered(object sender, EventArgs e)
        {
            this.ContentRendered -= ViewConnectionInfo_ContentRendered;
            
            double totalWidth = 0;
            foreach (GridViewColumn column in connectionGridView.Columns)
                totalWidth += column.ActualWidth + 7;
            this.Width = totalWidth;
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
