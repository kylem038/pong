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
	public bool readyPlayerTwo = false;

	private float _ballPosition;

	private bool upDirection = true;
	private float adjustedBallPosition;

	private float difficulty = 0.10F;

	public void Start(Vector2 position)
	{
		Position = position;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.Hit);
	}

	private void OnHudEnablePlayerTwo()
	{
		readyPlayerTwo = true;
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

	// Good reference to AI using a Prediction class
	// https://github.com/jakesgordon/javascript-pong/blob/master/part5/pong.js#L328
	private void AIControl(double delta)
	{
		var ball = GetNodeOrNull<RigidBody2D>("/root/Main/Ball");

		if (ball != null)
		{
			float windowSize = 32;
			float ballYPosition = ball.Position.Y;

			var lower = ballYPosition - windowSize;
			var upper = ballYPosition + windowSize;

			Vector2 window = new Vector2((float)lower, (float)upper);

			// on startup set _ballPosition once
			if (_ballPosition == 0)
			{
				_ballPosition = ballYPosition;
			}

			if (upDirection && _ballPosition > window.X) 
			{
				_ballPosition -= 1;
			} 
			else if (!upDirection && _ballPosition < window.Y)
			{	
				_ballPosition += 1;
			}
			else 
			{
                upDirection = !upDirection;
			}

			// Get difference between ball and drift
			adjustedBallPosition = (ballYPosition - _ballPosition) * difficulty;

			float finalBallPosition = ballYPosition + adjustedBallPosition;

			Position = new Vector2(
				x: Mathf.Clamp(Position.X, windowSize, ScreenSize.X - (float)windowSize),
				y: Mathf.Clamp(finalBallPosition, windowSize, ScreenSize.Y - (float)windowSize )
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
		if (readyPlayerTwo)
		{
			ControlPlayer(delta);
		}
		else
		{
			AIControl(delta);
		}

	}
}
