using System.Collections.ObjectModel;
using System.Windows.Input;
using Hypricing.Core.Services;
using Hypricing.HyprlangParser.Nodes;

namespace Hypricing.Desktop.ViewModels;

public sealed class StartupViewModel : ViewModelBase
{
    private readonly HyprlandService _service;
    private string? _statusMessage;
    private string _newCommand = string.Empty;
    private ExecVariant _newVariant = ExecVariant.Once;

    public StartupViewModel(HyprlandService service)
    {
        _service = service;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        AddCommand = new AsyncRelayCommand(AddEntryAsync);
    }

    public ObservableCollection<ExecItemViewModel> ExecEntries { get; } = [];

    public ICommand SaveCommand { get; }
    public ICommand AddCommand { get; }

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

    public string NewCommand
    {
        get => _newCommand;
        set
        {
            if (_newCommand == value) return;
            _newCommand = value;
            OnPropertyChanged();
        }
    }

    public ExecVariant NewVariant
    {
        get => _newVariant;
        set
        {
            if (_newVariant == value) return;
            _newVariant = value;
            OnPropertyChanged();
        }
    }

    public void Refresh()
    {
        ExecEntries.Clear();
        foreach (var exec in _service.GetExecEntries())
            ExecEntries.Add(new ExecItemViewModel(exec, RemoveEntry));
    }

    private void RemoveEntry(ExecItemViewModel item)
    {
        _service.RemoveExecEntry(item.Node);
        ExecEntries.Remove(item);
    }

    private async Task AddEntryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCommand))
            return;

        _service.AddExecEntry(NewVariant, NewCommand.Trim());
        NewCommand = string.Empty;

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
}
