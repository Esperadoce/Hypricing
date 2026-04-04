using System.Windows.Input;
using Hypricing.HyprlangParser.Nodes;

namespace Hypricing.Desktop.ViewModels;

/// <summary>
/// Wraps an <see cref="ExecNode"/> for data binding.
/// </summary>
public sealed class ExecItemViewModel : ViewModelBase
{
    private readonly ExecNode _node;
    private readonly Action<ExecItemViewModel>? _onRemove;

    public ExecItemViewModel(ExecNode node, Action<ExecItemViewModel>? onRemove = null)
    {
        _node = node;
        _onRemove = onRemove;
        RemoveCommand = new RelayCommand(() => _onRemove?.Invoke(this));
    }

    internal ExecNode Node => _node;
    public ICommand RemoveCommand { get; }

    public ExecVariant Variant
    {
        get => _node.Variant;
        set
        {
            if (_node.Variant == value) return;
            _node.Variant = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(VariantDisplay));
        }
    }

    public string Command
    {
        get => _node.Command;
        set
        {
            if (_node.Command == value) return;
            _node.Command = value;
            OnPropertyChanged();
        }
    }

    public string? Rules
    {
        get => _node.Rules;
        set
        {
            if (_node.Rules == value) return;
            _node.Rules = value;
            OnPropertyChanged();
        }
    }

    public string VariantDisplay => ExecNode.VariantToKeyword(_node.Variant);

    public static IReadOnlyList<ExecVariant> AllVariants { get; } =
    [
        ExecVariant.Once,
        ExecVariant.Reload,
        ExecVariant.OnceRestart,
        ExecVariant.ExecrReload,
        ExecVariant.Shutdown,
    ];
}
