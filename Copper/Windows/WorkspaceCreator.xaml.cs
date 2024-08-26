using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Copper
{
    /// <summary>
    /// Interaction logic for WorkspaceCreator.xaml
    /// </summary>
    public partial class WorkspaceCreator : Window
    {
        public MainWindow mainWindow;
        public WorkspaceCreator(MainWindow mainWindow)
        {
            Style = FindResource(typeof(Window)) as Style;

            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            if(mainWindow.Workspaces.ContainsKey(wsName.Text))
            {
                l1.Content = $"Workspace \"{wsName.Text}\" already exists.";
                return;
            }
            Grid newGrid = new Grid();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainWindow.Workspaces.Add(wsName.Text, newGrid);

            var old = mainWindow.Workspaces;
            mainWindow.Workspaces = new Dictionary<string, Grid>();
            mainWindow.Workspaces = old;

            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
