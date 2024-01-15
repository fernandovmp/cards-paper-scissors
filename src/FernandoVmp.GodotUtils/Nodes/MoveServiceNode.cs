using System.Collections.Concurrent;
using Godot;

namespace FernandoVmp.GodotUtils.Nodes;

public partial class MoveServiceNode : Node
{
    private ConcurrentDictionary<int, MoveToCommand> _commands = new ConcurrentDictionary<int, MoveToCommand>();
    private readonly List<int> _keysToRemove = new List<int>();

    public override void _Process(double delta)
    {
        float floatDelta = (float)delta;
        bool remove = false;

        foreach (var command in _commands.Values)
        {
            if(command.Completed) continue;
            command.Node.GlobalPosition = command.Node.GlobalPosition.MoveToward(command.TargetPosition, floatDelta * command.Speed);
            if (command.Node.GlobalPosition == command.TargetPosition)
            {
                command.Completed = true;
                remove = true;
                _keysToRemove.Add(command.Node.GetHashCode());
            }
        }

        if (remove)
        {
            foreach (var key in _keysToRemove)
            {
                _commands.Remove(key, out var command);
                command?.TaskCompletionSource?.SetResult();
            }
            _keysToRemove.Clear();
        }
    }

    public Task MoveToAsync(Node2D node, Vector2 targetPosition, float speed)
    {
        var command = new MoveToCommand(node, targetPosition, speed, new TaskCompletionSource());
        _commands.TryAdd(command.Node.GetHashCode(), command);
        return command.TaskCompletionSource!.Task;
    }
}

internal record MoveToCommand(
    Node2D Node,
    Vector2 TargetPosition,
    float Speed,
    TaskCompletionSource? TaskCompletionSource)
{
    internal bool Completed { get; set; }
};