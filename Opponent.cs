using Godot;

public partial class Opponent : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed { get; set; } = 250; // How fast the player will move.

	public Vector2 currentVelocity;

	// Size of the game window.
	public Vector2 ScreenSize;

	private float _ballPosition;

	private bool upDirection = true;

	public void Start(Vector2 position)
	{
		Position = position;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.Hit);
	}

	private void ControlPlayer(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_up_player_2"))
		{
			velocity.Y -= 1;
		}

		if (Input.IsActionPressed("move_down_player_2"))
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

	private void AIControl(double delta)
	{
		var ball = GetNodeOrNull<RigidBody2D>("/root/Main/Ball");

		if (ball != null)
		{
			// paddle height in px
			// var paddleSize = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
			double paddleSize = 32;
			float ballYPosition = ball.Position.Y;

			var lower = ballYPosition - paddleSize;
			var upper = ballYPosition + paddleSize;

			Vector2 window = new Vector2((float)lower, (float)upper);

			GD.Print("WINDOW Y: ", window.Y);
			GD.Print("WINDOW X: ", window.X);
			GD.Print("Ball POSITION: ", ballYPosition);
			GD.Print("UP DIRECTION?", upDirection);

			// on startup set _ballPosition once
			if (_ballPosition == 0)
			{
				_ballPosition = ballYPosition;
			}

			if (upDirection && _ballPosition > window.X) 
			{
				GD.Print("WE SUB 3");
				_ballPosition -= 1f;
			} 
			else if (!upDirection && _ballPosition < window.Y)
			{	
				GD.Print("WE ADD 3");
				_ballPosition += 1f;
			}
			else 
			{
				// modifiedYPosition = 0;
				GD.Print("TOGGLE DIRECTION OF WINDOW");
                upDirection = !upDirection;
			}

			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(_ballPosition, 0, ScreenSize.Y)
			);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool readyPlayer2 = false;
		if (readyPlayer2)
		{
			ControlPlayer(delta);
		}
		else
		{
			AIControl(delta);
		}

	}
}
