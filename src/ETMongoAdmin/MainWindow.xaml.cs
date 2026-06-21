using System.Windows;
using ETMongoAdmin.ViewModels;

namespace ETMongoAdmin;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
