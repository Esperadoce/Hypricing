using System.Collections.ObjectModel;
using System.Windows.Input;
using Hypricing.Core.Services;

namespace Hypricing.Desktop.ViewModels;

public sealed class VariablesViewModel : ViewModelBase
{
    private readonly HyprlandService _service;
    private string? _statusMessage;
    private string _newDeclName = string.Empty;
    private string _newDeclValue = string.Empty;
    private string _newEnvKey = string.Empty;
    private string _newEnvValue = string.Empty;

    public VariablesViewModel(HyprlandService service)
    {
        _service = service;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ReloadCommand = new AsyncRelayCommand(LoadAsync);
        AddDeclarationCommand = new AsyncRelayCommand(AddDeclarationAsync);
        AddEnvCommand = new AsyncRelayCommand(AddEnvAsync);
    }

    public ObservableCollection<DeclarationItemViewModel> Declarations { get; } = [];
    public ObservableCollection<EnvItemViewModel> EnvironmentVariables { get; } = [];

    public ICommand SaveCommand { get; }
    public ICommand ReloadCommand { get; }
    public ICommand AddDeclarationCommand { get; }
    public ICommand AddEnvCommand { get; }

    public string? StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value) return;
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public string NewDeclName
    {
        get => _newDeclName;
        set { if (_newDeclName != value) { _newDeclName = value; OnPropertyChanged(); } }
    }

    public string NewDeclValue
    {
        get => _newDeclValue;
        set { if (_newDeclValue != value) { _newDeclValue = value; OnPropertyChanged(); } }
    }

    public string NewEnvKey
    {
        get => _newEnvKey;
        set { if (_newEnvKey != value) { _newEnvKey = value; OnPropertyChanged(); } }
    }

    public string NewEnvValue
    {
        get => _newEnvValue;
        set { if (_newEnvValue != value) { _newEnvValue = value; OnPropertyChanged(); } }
    }

    public async Task LoadAsync()
    {
        try
        {
            await _service.LoadAsync();
            Refresh();
            StatusMessage = $"Loaded {Declarations.Count} declaration(s), {EnvironmentVariables.Count} env var(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load failed: {ex.Message}";
        }
    }

    private void RemoveDeclaration(DeclarationItemViewModel item)
    {
        _service.RemoveDeclaration(item.Node);
        Declarations.Remove(item);
    }

    private void RemoveEnv(EnvItemViewModel item)
    {
        _service.RemoveEnvironmentVariable(item.Node);
        EnvironmentVariables.Remove(item);
    }

    private async Task AddDeclarationAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDeclName)) return;
        _service.AddDeclaration(NewDeclName.Trim(), NewDeclValue.Trim());
        NewDeclName = string.Empty;
        NewDeclValue = string.Empty;
        await SaveAsync();
    }

    private async Task AddEnvAsync()
    {
        if (string.IsNullOrWhiteSpace(NewEnvKey)) return;
        _service.AddEnvironmentVariable(NewEnvKey.Trim(), NewEnvValue.Trim());
        NewEnvKey = string.Empty;
        NewEnvValue = string.Empty;
        await SaveAsync();
    }

    private async Task SaveAsync()
    {
        try
        {
            await _service.SaveAsync();
            Refresh();
            StatusMessage = "Saved and reloaded";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
        }
    }

    private void Refresh()
    {
        Declarations.Clear();
        foreach (var decl in _service.GetDeclarations())
            Declarations.Add(new DeclarationItemViewModel(decl, RemoveDeclaration));

        EnvironmentVariables.Clear();
        foreach (var env in _service.GetEnvironmentVariables())
            EnvironmentVariables.Add(new EnvItemViewModel(env, RemoveEnv));
    }
}
