using System;
using Godot;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler(string body);

	[Export]
	public int Speed { get; set; } = 250; // How fast the player will move.

	// Size of the game window.
	public Vector2 ScreenSize;

	public Vector2 currentVelocity; 

	private void OnBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.Hit, "player");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	public void Start(Vector2 position)
	{
		Position = position;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
		}

		currentVelocity = velocity;

		// This provides smooth movement even if the framerate changes
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}
}

