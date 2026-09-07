# Initial LibGodot Demo 
Shows the basic LibGodot usage. I tried to contain everything LibGodot specific to the `project/LibGodot` folder,
which is kind of huge, but thankfully largely self contained. And as it is kind of pain to write, this repo is MIT-0.

Also allows you to export LibGodot export template from a normal C# mono editor.

## Godot preparations
In `godot` folder:
```
scons target=editor module_mono_enabled=yes
<editor> --headless --generate-mono-glue modules/mono/glue
./modules/mono/build_scripts/build_assemblies.py --godot-output-dir ./bin --push-nupkgs-local ./../.local_nuget/
scons target=template_release module_mono_enabled=yes library_type=shared_library
```

## Project test
In `project` folder:
- Edit `LibGodot/LibGodot.props` if the OS is not Linux.
```
dotnet publish -c ExportRelease -p:BuildLibGodot=true
```
- Export the project, first skip native dotnet export, and then select the app from `LibGodot/template/`.
- Copy other files from `LibGodot/template/` to the export folder.
- Launch the game.

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

You should prefer to rely on the normal built in loading in editor, it will do everything for you by itself. Example:
```C#
internal unsafe static nint GetPluginInitialize()
{
#if TOOLS
    // Use builtin dotnet loading for editor.
    return (nint)(void*)null;
#else
    // Use default loader for the export build.
    return (nint)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, int, Godot.NativeInterop.godot_bool>)&global::GodotPlugins.Game.Main.InitializeFromGameProject;
#endif
}
```
If you still want to load it, you will need to load `InitializeFromEngine` function from the type `GodotPlugins.Main, GodotPlugins`
and return it. It should be located in `GodotSharp/Api/Debug/GodotPlugins.dll`. In a normal build you can use `Assembly.LoadFile`,
but in NativeAOT you will need to do the whole [native host thing](https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting)
to load it. As you can see the editor C# doesn't support NativeAOT, even if the loader uses it.

Also `GodotPlugins` will then load the Debug build of your current project from the memory, and yes, it will cause double loading,
there is basically nothing else to do if you want to still make it possible to support assembly reloading. So, once again, 
I don't recommend Editor LibGodot.
