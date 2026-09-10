#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Sample;

[Tool]
public partial class DisableNativeExport : EditorPlugin
{
	LibGodotExportPlugin? exportPlugin = null;
	public override void _EnterTree()
	{
		RemoveNativeExportPlugin();

		exportPlugin = new();
		AddExportPlugin(exportPlugin);
	}

	public override void _ExitTree()
	{
		RemoveExportPlugin(exportPlugin);
		exportPlugin = null;
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

[Tool]
public partial class LibGodotExportPlugin : EditorExportPlugin
{
	public override string _GetName() => "LibGodotExport";
	public override bool _SupportsPlatform(EditorExportPlatform platform) => true;
	public override string[] _GetExportFeatures(EditorExportPlatform platform, bool debug)
	{
		return ["dotnet"];
	}

	public override void _ExportFile(string path, string type, string[] features)
	{
		if (type != "CSharpScript")
			return;
		AddFile(path, System.Text.Encoding.UTF8.GetBytes("\n"), remap: false);
		Skip();
	}
	public override void _ExportBegin(string[] features, bool isDebug, string path, uint flags)
	{
		string sourcePath = "res://LibGodot/template";
		string targetPath = path.GetBaseDir();

		using DirAccess? source = DirAccess.Open(sourcePath);
		using DirAccess? target = DirAccess.Open(targetPath);
		if (source is not null && target is not null)
		{
			string[] files = source.GetFiles();

			foreach (string file in files)
			{
				if (file == path.GetFile())
					continue;

				using FileAccess tfa = FileAccess.Open($"{targetPath}/{file}", FileAccess.ModeFlags.Write);
				tfa.StoreBuffer(FileAccess.GetFileAsBytes($"{sourcePath}/{file}"));
				tfa.Close();
			}
		}
	}
}
#endif
