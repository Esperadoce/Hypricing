using System.Collections.ObjectModel;
using System.Windows.Input;
using Hypricing.Core.Services;

namespace Hypricing.Desktop.ViewModels;

public sealed class VariablesViewModel : ViewModelBase
{
    private readonly HyprlandService _service;
    private string? _statusMessage;

    public VariablesViewModel(HyprlandService service)
    {
        _service = service;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ReloadCommand = new AsyncRelayCommand(LoadAsync);
    }

    public ObservableCollection<DeclarationItemViewModel> Declarations { get; } = [];
    public ObservableCollection<EnvItemViewModel> EnvironmentVariables { get; } = [];

    public ICommand SaveCommand { get; }
    public ICommand ReloadCommand { get; }

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
            Declarations.Add(new DeclarationItemViewModel(decl));

        EnvironmentVariables.Clear();
        foreach (var env in _service.GetEnvironmentVariables())
            EnvironmentVariables.Add(new EnvItemViewModel(env));
    }

}
