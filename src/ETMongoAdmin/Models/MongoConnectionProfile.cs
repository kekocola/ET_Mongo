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
    private bool _useSshProxy;
    private string _sshHost = string.Empty;
    private string _sshPort = "22";
    private string _sshUserName = string.Empty;
    private string _sshPassword = string.Empty;
    private string _sshPrivateKeyPath = string.Empty;

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

    public bool UseSshProxy
    {
        get => _useSshProxy;
        set
        {
            if (SetField(ref _useSshProxy, value))
            {
                OnPropertyChanged(nameof(SshProxySummary));
            }
        }
    }

    public string SshHost
    {
        get => _sshHost;
        set
        {
            if (SetField(ref _sshHost, value))
            {
                OnPropertyChanged(nameof(SshProxySummary));
            }
        }
    }

    public string SshPort
    {
        get => _sshPort;
        set
        {
            if (SetField(ref _sshPort, value))
            {
                OnPropertyChanged(nameof(SshProxySummary));
            }
        }
    }

    public string SshUserName
    {
        get => _sshUserName;
        set
        {
            if (SetField(ref _sshUserName, value))
            {
                OnPropertyChanged(nameof(SshProxySummary));
            }
        }
    }

    public string SshPassword
    {
        get => _sshPassword;
        set => SetField(ref _sshPassword, value);
    }

    public string SshPrivateKeyPath
    {
        get => _sshPrivateKeyPath;
        set => SetField(ref _sshPrivateKeyPath, value);
    }

    public string Endpoint => string.Concat(Host, ":", Port);

    public string SshProxySummary
    {
        get
        {
            if (!UseSshProxy)
            {
                return "未启用 SSH 代理";
            }

            var userPrefix = string.IsNullOrWhiteSpace(SshUserName) ? string.Empty : string.Concat(SshUserName, "@");
            return string.Concat("SSH：", userPrefix, SshHost, ":", SshPort);
        }
    }

    public MongoConnectionProfile Clone()
    {
        return new MongoConnectionProfile
        {
            Id = Id,
            Name = Name,
            Host = Host,
            Port = Port,
            Database = Database,
            UserName = UserName,
            UseSshProxy = UseSshProxy,
            SshHost = SshHost,
            SshPort = SshPort,
            SshUserName = SshUserName,
            SshPassword = SshPassword,
            SshPrivateKeyPath = SshPrivateKeyPath
        };
    }

    public MongoConnectionProfile CloneAsCopy()
    {
        var copy = Clone();
        copy.Id = Guid.NewGuid().ToString("N");
        copy.Name = string.IsNullOrWhiteSpace(Name) ? "未命名连接 - 副本" : string.Concat(Name, " - 副本");
        return copy;
    }

    public void CopyFrom(MongoConnectionProfile source)
    {
        Name = source.Name;
        Host = source.Host;
        Port = source.Port;
        Database = source.Database;
        UserName = source.UserName;
        UseSshProxy = source.UseSshProxy;
        SshHost = source.SshHost;
        SshPort = source.SshPort;
        SshUserName = source.SshUserName;
        SshPassword = source.SshPassword;
        SshPrivateKeyPath = source.SshPrivateKeyPath;
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
