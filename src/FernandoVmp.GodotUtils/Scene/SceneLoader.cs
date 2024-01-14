using Godot;

namespace FernandoVmp.GodotUtils.Scene;

public static class SceneLoader
{
    public static void LoadInto(Node root, string scenePath)
    {
        var scene = ResourceLoader.Load<PackedScene>(scenePath);
        LoadInto(root, scene);
    }
    
    public static void LoadInto(Node root, PackedScene packedScene)
    {
        var instance = packedScene.Instantiate();
        if(root.GetChildCount() > 0)
            root.RemoveChild(root.GetChild(0));
        root.AddChild(instance);
    }
}