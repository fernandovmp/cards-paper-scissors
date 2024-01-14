using Godot;

namespace FernandoVmp.GodotUtils.Nodes;

public partial class MoveServiceNode : Node
{
    private List<MoveToCommand> _commands = new List<MoveToCommand>();

    public override void _Process(double delta)
    {
        float floatDelta = (float)delta;
        bool remove = false;
        
        foreach (var command in _commands)
        {
            if(command.Completed) continue;
            command.Node.GlobalPosition = command.Node.GlobalPosition.MoveToward(command.TargetPosition, floatDelta * command.Speed);
            if (command.Node.GlobalPosition == command.TargetPosition)
            {
                command.TaskCompletionSource?.SetResult();
                command.Completed = true;
                remove = true;
            }
        }

        if (remove)
        {
            _commands.RemoveAll(cmd => cmd.Completed);
        }
    }

    public Task MoveToAsync(Node2D node, Vector2 targetPosition, float speed)
    {
        var command = new MoveToCommand(node, targetPosition, speed, new TaskCompletionSource());
        _commands.Add(command);
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