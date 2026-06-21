using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ETMongoAdmin.Models;

namespace ETMongoAdmin.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private MongoConnectionProfile? _selectedConnection;
    private MongoConnectionProfile? _activeConnection;
    private MongoConnectionProfile _editingConnection = new();
    private bool _isEditorOpen;
    private bool _isEditingExisting;
    private string _editorError = string.Empty;

    public MainWindowViewModel()
    {
        Connections.CollectionChanged += OnConnectionsChanged;

        AddConnectionCommand = new RelayCommand(AddConnection);
        EditConnectionCommand = new RelayCommand(EditConnection, () => SelectedConnection is not null);
        DeleteConnectionCommand = new RelayCommand(DeleteConnection, () => SelectedConnection is not null);
        OpenWorkspaceCommand = new RelayCommand(OpenWorkspace, () => SelectedConnection is not null);
        DisconnectCommand = new RelayCommand(Disconnect, () => IsWorkspaceOpen);
        SaveConnectionCommand = new RelayCommand(SaveConnection);
        CancelEditCommand = new RelayCommand(CancelEdit);
    }

    public ObservableCollection<MongoConnectionProfile> Connections { get; } = new();

    public MongoConnectionProfile? SelectedConnection
    {
        get => _selectedConnection;
        set
        {
            if (SetField(ref _selectedConnection, value))
            {
                RaiseCommandStates();
                OnPropertyChanged(nameof(WorkspaceTitle));
            }
        }
    }

    public MongoConnectionProfile? ActiveConnection
    {
        get => _activeConnection;
        private set
        {
            if (SetField(ref _activeConnection, value))
            {
                OnPropertyChanged(nameof(IsWorkspaceOpen));
                OnPropertyChanged(nameof(IsWorkspaceClosed));
                OnPropertyChanged(nameof(WorkspaceTitle));
                RaiseCommandStates();
            }
        }
    }

    public MongoConnectionProfile EditingConnection
    {
        get => _editingConnection;
        private set => SetField(ref _editingConnection, value);
    }

    public bool IsEditorOpen
    {
        get => _isEditorOpen;
        private set => SetField(ref _isEditorOpen, value);
    }

    public string EditorTitle => _isEditingExisting ? "编辑 MongoDB 连接" : "新增 MongoDB 连接";

    public string EditorError
    {
        get => _editorError;
        private set => SetField(ref _editorError, value);
    }

    public bool HasNoConnections => Connections.Count == 0;

    public bool IsWorkspaceOpen => ActiveConnection is not null;

    public bool IsWorkspaceClosed => !IsWorkspaceOpen;

    public string WorkspaceTitle
    {
        get
        {
            if (ActiveConnection is not null)
            {
                return string.Concat("当前工作区：", ActiveConnection.Name, " (", ActiveConnection.Endpoint, ")");
            }

            return SelectedConnection is null
                ? "尚未连接"
                : string.Concat("已选择：", SelectedConnection.Name);
        }
    }

    public ICommand AddConnectionCommand { get; }
    public ICommand EditConnectionCommand { get; }
    public ICommand DeleteConnectionCommand { get; }
    public ICommand OpenWorkspaceCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand SaveConnectionCommand { get; }
    public ICommand CancelEditCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void AddConnection()
    {
        _isEditingExisting = false;
        EditingConnection = new MongoConnectionProfile
        {
            Name = "本地 Mongo",
            Host = "localhost",
            Port = "27017",
            Database = "ETGame"
        };
        EditorError = string.Empty;
        IsEditorOpen = true;
        OnPropertyChanged(nameof(EditorTitle));
    }

    private void EditConnection()
    {
        if (SelectedConnection is null)
        {
            return;
        }

        _isEditingExisting = true;
        EditingConnection = SelectedConnection.Clone();
        EditorError = string.Empty;
        IsEditorOpen = true;
        OnPropertyChanged(nameof(EditorTitle));
    }

    private void DeleteConnection()
    {
        if (SelectedConnection is null)
        {
            return;
        }

        var deleting = SelectedConnection;
        Connections.Remove(deleting);

        if (ActiveConnection?.Id == deleting.Id)
        {
            ActiveConnection = null;
        }

        SelectedConnection = Connections.FirstOrDefault();
    }

    private void OpenWorkspace()
    {
        if (SelectedConnection is null)
        {
            return;
        }

        ActiveConnection = SelectedConnection;
    }

    private void Disconnect()
    {
        ActiveConnection = null;
    }

    private void SaveConnection()
    {
        var validationMessage = Validate(EditingConnection);
        if (!string.IsNullOrWhiteSpace(validationMessage))
        {
            EditorError = validationMessage;
            return;
        }

        if (_isEditingExisting)
        {
            var target = Connections.FirstOrDefault(item => item.Id == EditingConnection.Id);
            target?.CopyFrom(EditingConnection);
            SelectedConnection = target;
        }
        else
        {
            EditingConnection.Id = Guid.NewGuid().ToString("N");
            Connections.Add(EditingConnection);
            SelectedConnection = EditingConnection;
        }

        EditorError = string.Empty;
        IsEditorOpen = false;
    }

    private void CancelEdit()
    {
        EditorError = string.Empty;
        IsEditorOpen = false;
    }

    private static string Validate(MongoConnectionProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Name))
        {
            return "请输入连接名称。";
        }

        if (string.IsNullOrWhiteSpace(profile.Host))
        {
            return "请输入 MongoDB Host。";
        }

        if (!int.TryParse(profile.Port, out var port) || port <= 0 || port > 65535)
        {
            return "请输入有效的端口号（1-65535）。";
        }

        if (string.IsNullOrWhiteSpace(profile.Database))
        {
            return "请输入默认数据库名称。";
        }

        return string.Empty;
    }

    private void OnConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasNoConnections));
        RaiseCommandStates();
    }

    private void RaiseCommandStates()
    {
        (EditConnectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteConnectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (OpenWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DisconnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
