using System.Windows;
using System.Windows.Controls;
using ETMongoAdmin.Models;
using ETMongoAdmin.ViewModels;

namespace ETMongoAdmin;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var viewModel = new MainWindowViewModel
        {
            ConfirmDeleteConnection = ConfirmDeleteConnection
        };
        DataContext = viewModel;
    }

    private void ConnectionMenuButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.ContextMenu is null)
        {
            return;
        }

        button.ContextMenu.PlacementTarget = button;
        button.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void ConnectionItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel ||
            sender is not FrameworkElement { DataContext: MongoConnectionProfile connection } ||
            !viewModel.OpenWorkspaceCommand.CanExecute(connection))
        {
            return;
        }

        viewModel.OpenWorkspaceCommand.Execute(connection);
        e.Handled = true;
    }

    private bool ConfirmDeleteConnection(MongoConnectionProfile connection)
    {
        var result = MessageBox.Show(
            this,
            $"确定要删除连接“{connection.Name}”吗？\n\n删除后当前内存中的连接配置将被移除。",
            "确认删除连接",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
}
