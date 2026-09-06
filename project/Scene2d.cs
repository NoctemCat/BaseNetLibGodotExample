using Godot;
using System;

public partial class Scene2d : Node2D
{
	[Export] private Slot2d[]? slots;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Slots ", (Variant)(slots ?? new Variant()));
	}
}
