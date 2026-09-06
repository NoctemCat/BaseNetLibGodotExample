#nullable enable
// using ;
using LibGodot.Bridge;
using Sample;
using Xunit.Abstractions;

namespace LibGodotXUnit;

public class GodotFixture : IDisposable
{
    LibGodot.Bridge.GodotInstance? instance = null;

    public GodotFixture()
    {
        List<string> args = [.. Environment.GetCommandLineArgs()];
        bool justMainPack = true;
        args.InsertRange(1, ["--disable-crash-handler", "--headless"]);
        // args.InsertRange(2, ["--main-pack", "LibGodotXUnit.pck"]); // Just main pack.
        // args.InsertRange(2, ["--script", "<script>"]); // Also possible to use script.

        justMainPack = false;
        args.InsertRange(2, ["--main-pack", "LibGodotXUnit.pck", "--scene", "res://TestScene.tscn"]);

        instance = LibGodot.Bridge.LibGodot.CreateGodotInstance(args) ?? throw new ApplicationException("Error creating Godot instance.");
        LibGodot.Bridge.LibGodot.SetPluginsInitialize(Initializer.GetPluginInitialize());

        instance.Start();

        if (justMainPack)
        {
            var isInit = typeof(Godot.NativeInterop.NativeFuncs)?.GetMethod("godotsharp_dotnet_module_is_initialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Godot.NativeInterop.godot_bool gbool = Godot.NativeInterop.GodotBoolExtensions.ToGodotBool(false);
            while (!Godot.NativeInterop.GodotBoolExtensions.ToBool(gbool))
            {
                gbool = (Godot.NativeInterop.godot_bool)(isInit?.Invoke(null, null) ?? default(Godot.NativeInterop.godot_bool));
                if (instance.Iteration())
                {
                    Cleanup();
                    throw new ApplicationException("Godot exited in initialization.");
                }
            }
        }
        else
        {
            while (!TestScene.SceneReady)
            {
                if (instance.Iteration())
                {
                    Cleanup();
                    throw new ApplicationException("Godot exited before scene is ready.");
                }
            }
        }

    }

    ~GodotFixture() => Cleanup();


    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Cleanup();
    }

    private void Cleanup()
    {
        instance?.Dispose();
        instance = null;
    }
}

[CollectionDefinition("Godot collection")]
public class GodotCollection : ICollectionFixture<GodotFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}


[Collection("Godot collection")]
public class SimpleUnitTest
{
    private readonly ITestOutputHelper output;

    // xUnit injects this helper automatically
    public SimpleUnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void SetNodeName()
    {
        Godot.Node node = new()
        {
            Name = "CustomName"
        };
        Assert.Equal("CustomName", node.Name);
        node.Free();
    }

    [Fact]
    public void CompareStringName()
    {
        Godot.StringName name1 = "Name 1";
        Godot.StringName name2 = "Name 2";
        Godot.StringName name3 = "Name 1";

        Assert.Equal(name1, name3);
        Assert.NotEqual(name1, name2);
    }

    [Fact]
    public void LoadResource()
    {
        var resource = Godot.GD.Load<ResourceData>("res://Data.tres");

        Assert.True(resource.Table.ContainsKey(55));
        Assert.Equal("Hello", resource.Table[55]);
        Assert.Equal("World", resource.Table[101]);
        Assert.False(resource.Table.ContainsKey(1));

        var scene = resource.SceneExport.Instantiate();
        var player = scene.GetNodeOrNull<Player>("Player");
        Assert.True(player is not null);
        scene.Free();
    }
}
