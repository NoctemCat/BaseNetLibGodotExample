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