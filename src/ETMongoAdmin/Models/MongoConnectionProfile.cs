using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETMongoAdmin.Models;

public sealed class MongoConnectionProfile : INotifyPropertyChanged
{
    private string _id = Guid.NewGuid().ToString("N");
    private string _name = string.Empty;
    private string _host = "localhost";
    private string _port = "27017";
    private string _database = string.Empty;
    private string _userName = string.Empty;

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Host
    {
        get => _host;
        set
        {
            if (SetField(ref _host, value))
            {
                OnPropertyChanged(nameof(Endpoint));
            }
        }
    }

    public string Port
    {
        get => _port;
        set
        {
            if (SetField(ref _port, value))
            {
                OnPropertyChanged(nameof(Endpoint));
            }
        }
    }

    public string Database
    {
        get => _database;
        set => SetField(ref _database, value);
    }

    public string UserName
    {
        get => _userName;
        set => SetField(ref _userName, value);
    }

    public string Endpoint => string.Concat(Host, ":", Port);

    public MongoConnectionProfile Clone()
    {
        return new MongoConnectionProfile
        {
            Id = Id,
            Name = Name,
            Host = Host,
            Port = Port,
            Database = Database,
            UserName = UserName
        };
    }

    public void CopyFrom(MongoConnectionProfile source)
    {
        Name = source.Name;
        Host = source.Host;
        Port = source.Port;
        Database = source.Database;
        UserName = source.UserName;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
