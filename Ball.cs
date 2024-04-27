using Godot;

public partial class Ball : RigidBody2D
{
	[Signal]
	public delegate void OnScoreEventHandler(string scorer);

	public void Place(Vector2 position)
	{
		Position = position;
	}

	public void OnHit(string body)
	{
		// Play pingpong sound
		GetNode<AudioStreamPlayer>("/root/Main/PingPong").Play();

		// get current velocity
		var currentXVelocity = LinearVelocity.X;
		var currentYVelocity = LinearVelocity.Y;

		// Add "spin" to the ball
		if(body == "opponent")
		{
			var opponent = GetNode<Opponent>("/root/Main/Opponent");
			float paddleVelocity = opponent.currentVelocity.Y;

			// if ball is moving up and paddle is moving up
			if (currentYVelocity < 0 && paddleVelocity != 0 && paddleVelocity < 0) {
				currentXVelocity -= paddleVelocity;
			}
			// if ball is moving down and paddle is moving down, add velocity
			if (currentYVelocity > 0 && paddleVelocity != 0 && paddleVelocity > 0) {
				currentXVelocity += paddleVelocity;
			}
			// if ball is moving up and paddle is moving down, subtract velocity
			if (currentYVelocity < 0 && paddleVelocity != 0 && paddleVelocity > 0) {
				currentXVelocity -= paddleVelocity;
			}
			// if ball is moving down and paddle is moving up, subtract
			if (currentYVelocity > 0 && paddleVelocity != 0 && paddleVelocity < 0) {
				currentXVelocity += paddleVelocity;
			}
		}
		else
		{
			var player = GetNode<Player>("/root/Main/Player");
			float paddleVelocity = player.currentVelocity.Y;

			if (currentYVelocity < 0 && paddleVelocity != 0 && paddleVelocity < 0) {
				currentXVelocity += paddleVelocity;
			}
			// if ball is moving down and paddle is moving down
			if (currentYVelocity > 0 && paddleVelocity != 0 && paddleVelocity > 0) {
				currentXVelocity -= paddleVelocity;
			}
			// if ball is moving up and paddle is moving down
			if (currentYVelocity < 0 && paddleVelocity != 0 && paddleVelocity > 0) {
				float dampedVelocity = paddleVelocity;
				currentXVelocity += dampedVelocity;
			}
			// if ball is moving down and paddle is moving up
			if (currentYVelocity > 0 && paddleVelocity != 0 && paddleVelocity < 0) {
				float dampedVelocity = paddleVelocity;
				currentXVelocity -= dampedVelocity;
			}
		}

		// Prevent ball from moving TOO fast OR slow
		currentXVelocity = body == "opponent" ? 
			Mathf.Clamp(currentXVelocity, 275, 600) : Mathf.Clamp(currentXVelocity, -600, -275);

		// set the velocity to the opposite X & Y
		// Rotate just a bit to add some variance to exit vector
		LinearVelocity = new Vector2(-currentXVelocity, currentYVelocity)
			.Rotated((float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4));;
	}

	// How can I not have to pass body here? It's not needed
	public void OnWallBodyEntered(Node2D body)
	{
		// get current velocity
		var currentXVelocity = LinearVelocity.X;
		var currentYVelocity = LinearVelocity.Y;

		// set the velocity to the opposite X & Y
		LinearVelocity = new Vector2(currentXVelocity, -currentYVelocity);
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		// Clean up signal connections
		DisconnectSignals();

		// Determine which side of the screen was exited
		// Set who scored
		var scorer = Position.X > 0 ? "player" : "opponent";

		// Despawn ball after it leave the screen
		QueueFree();
		EmitSignal(SignalName.OnScore, scorer);
	}

	// Good example of connecting and disconnecting signals
	// For instanced scenes
	private void ConnectSignals()
	{
		var player = GetNode<Player>("/root/Main/Player");
		var opponent = GetNode<Opponent>("/root/Main/Opponent");
		var topWall = GetNode<Area2D>("/root/Main/TopWall");
		var bottomWall = GetNode<Area2D>("/root/Main/BottomWall");
		player.Hit += OnHit;
		opponent.Hit += OnHit;
		topWall.BodyEntered += OnWallBodyEntered;
		bottomWall.BodyEntered += OnWallBodyEntered;
	}

	public void DisconnectSignals()
	{
		var player = GetNode<Player>("/root/Main/Player");
		var opponent = GetNode<Opponent>("/root/Main/Opponent");
		var topWall = GetNode<Area2D>("/root/Main/TopWall");
		var bottomWall = GetNode<Area2D>("/root/Main/BottomWall");
		player.Hit -= OnHit;
		opponent.Hit -= OnHit;
		topWall.BodyEntered -= OnWallBodyEntered;
		bottomWall.BodyEntered -= OnWallBodyEntered;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ConnectSignals();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
