# Initial LibGodot Demo 
Shows the basic LibGodot usage. I tried to contain everything LibGodot specific to the `project/LibGodot` folder,
which is kind of huge, but thankfully largely self contained. And as it is kind of pain to write, this repo is MIT-0.

Also allows you to export LibGodot export template from a normal C# mono editor.

## How to build

### Godot preparations
In `godot` folder:
```
scons target=editor module_mono_enabled=yes
<editor> --headless --generate-mono-glue modules/mono/glue
./modules/mono/build_scripts/build_assemblies.py --godot-output-dir ./bin --push-nupkgs-local ./../.local_nuget/
scons target=template_release module_mono_enabled=yes library_type=shared_library
```

### Project test
In `project` folder:
- Edit `LibGodot/LibGodot.props` if the OS is not Linux.
```
dotnet publish -c ExportRelease -p:BuildLibGodot=true
```
- Export the project, then select the app from `LibGodot/template/`.
- Copy other files from `LibGodot/template/` to the export folder, this repo uses `EditorExportPlugin` to do it.
- Launch the game.

### Tested with
```
scons target=editor module_mono_enabled=yes accesskit=no
./bin/godot.linuxbsd.editor.x86_64.mono --headless --generate-mono-glue modules/mono/glue
./modules/mono/build_scripts/build_assemblies.py --godot-output-dir ./bin --push-nupkgs-local ./../.local_nuget/
scons target=template_release module_mono_enabled=yes library_type=shared_library accesskit=no
scons target=template_debug module_mono_enabled=yes library_type=shared_library accesskit=no disable_path_overrides=no
```
```
dotnet publish -c ExportRelease -p:BuildLibGodot=true
```

## Workarounds

### Disable native C# export plugin

You can use reflection to disable it. The exported project will need to have feature tag `"dotnet"` to initialize the C# module.
You can add it yourself in a feature tab when exporting or through `EditorExportPlugin._GetExportFeatures`.

Code:
```C#
[Tool]
public partial class DisableNativeExport : EditorPlugin
{
	public override void _EnterTree()
	{
		RemoveNativeExportPlugin();
	}

	public void RemoveNativeExportPlugin()
	{
		var editorAssembly = AppDomain.CurrentDomain
					.GetAssemblies()
					.First(x => x.GetName().Name == "GodotTools");
		var sharpEditor = editorAssembly.GetType("GodotTools.GodotSharpEditor");

		PropertyInfo? miInstance = (PropertyInfo?)sharpEditor?.GetMember("Instance", BindingFlags.Static | BindingFlags.Public).FirstOrDefault();
		FieldInfo? miExportPlugin = (FieldInfo?)sharpEditor?.GetMember("_exportPluginWeak", BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
		var gseInstance = miInstance?.GetValue(null);
		var sharpExportPluginRef = (WeakRef?)miExportPlugin?.GetValue(gseInstance);
		var sharpExportPlugin = sharpExportPluginRef?.GetRef().As<EditorExportPlugin>();
		if (sharpExportPlugin is not null)
		{
			RemoveExportPlugin(sharpExportPlugin);
		}
	}
}
```

### Create plugins init entry point

You will need to provide the initialization entry point, `NativeAOT` will need a fully custom one (for some reason
reflection doesn't work at all) or you can use `MethodHandle.GetFunctionPointer()` to grap the generated one.

```c#
// Copy of `InitializeFromGameProject` added by `GodotPluginsInitializerGenerator`.
[UnmanagedCallersOnly]
private static Godot.NativeInterop.godot_bool InitializeFromLibGodot(IntPtr godotDllHandle, IntPtr outManagedCallbacks,
	IntPtr unmanagedCallbacks, int unmanagedCallbacksSize)
{
	try
	{
		DllImportResolver dllImportResolver = new Godot.NativeInterop.GodotDllImportResolver(godotDllHandle).OnResolveDllImport;
		var coreApiAssembly = typeof(global::Godot.GodotObject).Assembly;
		NativeLibrary.SetDllImportResolver(coreApiAssembly, dllImportResolver);
		Godot.NativeInterop.NativeFuncs.Initialize(unmanagedCallbacks, unmanagedCallbacksSize);
		Godot.Bridge.ManagedCallbacks.Create(outManagedCallbacks);
		Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(typeof(global::GodotPlugins.Game.Main).Assembly);
		return Godot.NativeInterop.godot_bool.True;
	}
	catch (Exception e)
	{
		global::System.Console.Error.WriteLine(e);
		return Godot.NativeInterop.GodotBoolExtensions.ToGodotBool(false);
	}
}

internal unsafe static nint GetPluginInitialize()
{
#if TOOLS
	// Use builtin dotnet loading for editor.
	return (nint)(void*)null;
#else
	// Use default loader for the export build.
	return (nint)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, int, Godot.NativeInterop.godot_bool>)&InitializeFromLibGodot;
	// Doesn't work in NativeAOT.
	// return (nint)(void*)typeof(GodotPlugins.Game.Main).GetMethod("InitializeFromGameProject", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer();
#endif
}
```

## Nuget
`--push-nupkgs-local ./../.local_nuget/` pushes packages to `.local_nuget` inside root, 
`project/nuget.config` adds it as a source so a `.csproj` can find it

## xUnit.net
Needs `disable_path_overrides=no`, only supports testing on export templates either with `--main-pack <file>` or with `--script <script>`.
Also the repo uses `template_debug`.
In `project/LobGodotXUnit` folder:
- Edit package path and the shared library path in `LibGodotXUnit.csproj`.
```
dotnet test -c ExportDebug
```
Currently there is no plans to add editor support for testing as the editor LibGodot still needs to be able to reload assemblies, and
it works through loading the C# project fully from memory. Double loading here is a feature.

## Editor LibGodot, for some reason
This section is for people who like to write bootstrap code by themselves. It's not really recommended.

You should prefer to rely on the normal built in loading in editor, it will do everything for you by itself.

If you still want to load it, you will need to load `InitializeFromEngine` function from the type `GodotPlugins.Main, GodotPlugins`
and return it. It should be located in `GodotSharp/Api/Debug/GodotPlugins.dll`. In a normal build you can use `Assembly.LoadFile`,
but in NativeAOT you will need to do the whole [native host thing](https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting)
to load it. As you can see the editor C# doesn't support NativeAOT, even if the loader uses it.

Also `GodotPlugins` will then load the Debug build of your current project from the memory, and yes, it will cause double loading,
there is basically nothing else to do if you want to still make it possible to support assembly reloading. So, once again, 
I don't recommend Editor LibGodot.
