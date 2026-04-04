using System.Windows.Input;
using Hypricing.HyprlangParser.Nodes;

namespace Hypricing.Desktop.ViewModels;

/// <summary>
/// Wraps a single <see cref="DeclarationNode"/> for data binding.
/// Writes back to the underlying AST node on change.
/// </summary>
public sealed class DeclarationItemViewModel : ViewModelBase
{
    private readonly DeclarationNode _node;

    public DeclarationItemViewModel(DeclarationNode node, Action<DeclarationItemViewModel>? onRemove = null)
    {
        _node = node;
        RemoveCommand = new RelayCommand(() => onRemove?.Invoke(this));
    }

    internal DeclarationNode Node => _node;
    public ICommand RemoveCommand { get; }

    public string Name
    {
        get => _node.Name;
        set
        {
            if (_node.Name == value) return;
            _node.Name = value;
            OnPropertyChanged();
        }
    }

    public string Value
    {
        get => _node.Value;
        set
        {
            if (_node.Value == value) return;
            _node.Value = value;
            OnPropertyChanged();
        }
    }
}
