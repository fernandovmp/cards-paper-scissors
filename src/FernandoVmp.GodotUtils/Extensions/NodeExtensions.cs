using Godot;

namespace FernandoVmp.GodotUtils.Extensions;

public static class NodeExtensions
{
    public static SignalAwaiter WaitForSeconds(this Node node, double seconds)
        => node.ToSignal(node.GetTree().CreateTimer(seconds), "timeout");
}