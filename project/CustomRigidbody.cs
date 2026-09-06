using System;
using Godot;

namespace Sample;

public partial class CustomRigidbody : RigidBody3D
{
	public override void _Ready()
	{
		GD.Print("Creating RigidBody3D");
	}
}
