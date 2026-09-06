using Godot;
using System;

namespace Sample;

public partial class TestScene : Node
{
	public static bool SceneReady { get; private set; } = false;

	public override void _Ready()
	{
		SceneReady = true;
	}
}
