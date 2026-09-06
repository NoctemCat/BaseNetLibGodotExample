using Godot;
using Godot.Collections;
using System;

namespace Sample;

[GlobalClass]
public partial class ResourceData : Resource
{
	[Export] public Dictionary<int, string> Table { get; set; } = null!;
	[Export] public PackedScene SceneExport { get; set; } = null!;
}
