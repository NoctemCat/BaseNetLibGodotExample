using Godot;
using System;

namespace Sample;

public partial class MovePlayer : MeshInstance3D
{
	[Export(PropertyHint.File, "*.tscn")] string PathToLevel = "";
	[Export] Area3D area3D = null!;

	public override void _Ready()
	{
		area3D.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node3D _body)
	{
		if (_body is Player)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, PathToLevel);
		}
	}
}
