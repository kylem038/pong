using Godot;

public partial class Ball : RigidBody2D
{
	[Signal]
	public delegate void OnScoreEventHandler(string scorer);

	public void Place(Vector2 position)
	{
		Position = position;
	}

	public void OnOpponentHit()
	{
		// Play pingpong sound
		GetNode<AudioStreamPlayer>("/root/Main/PingPong").Play();

		// get current velocity
		var currentXVelocity = LinearVelocity.X;
		var currentYVelocity = LinearVelocity.Y;

		// get velocity of the paddle
		var opponent = GetNode<Opponent>("/root/Main/Opponent");
		var opponentPaddleVelocity = opponent.currentVelocity.Y;
		// if ball is moving up and paddle is moving up
		if (currentYVelocity < 0 && opponentPaddleVelocity != 0 && opponentPaddleVelocity < 0) {
			GD.Print("PADDLE UP & BALL UP");
			currentXVelocity -= opponentPaddleVelocity;
		}
		// if ball is moving down and paddle is moving down, add velocity
		if (currentYVelocity > 0 && opponentPaddleVelocity != 0 && opponentPaddleVelocity > 0) {
			GD.Print("PADDLE DOWN & BALL DOWN");
			currentXVelocity += opponentPaddleVelocity;
		}
		// if ball is moving up and paddle is moving down, subtract velocity
		if (currentYVelocity < 0 && opponentPaddleVelocity != 0 && opponentPaddleVelocity > 0) {
			GD.Print("PADDLE DOWN & BALL UP");
			float dampedVelocity = opponentPaddleVelocity;
			currentXVelocity -= dampedVelocity;
		}
		// if ball is moving down and paddle is moving up, subtract
		if (currentYVelocity > 0 && opponentPaddleVelocity != 0 && opponentPaddleVelocity < 0) {
			GD.Print("PADDLE UP & BALL DOWN");
			float dampedVelocity = opponentPaddleVelocity;
			currentXVelocity += dampedVelocity;
		}

		// Prevent ball from moving TOO fast OR slow
		currentXVelocity = Mathf.Clamp(currentXVelocity, 275, 600);

		// set the velocity to the opposite X & Y
		// Rotate just a bit to add some variance to exit vector
		LinearVelocity = new Vector2(-currentXVelocity, currentYVelocity)
			.Rotated((float)GD.RandRange(-Mathf.Pi / 6, Mathf.Pi / 6));;
	}

	public void OnPlayerHit()
	{
		// Play pingpong sound
		GetNode<AudioStreamPlayer>("/root/Main/PingPong").Play();

		// get current velocity
		var currentXVelocity = LinearVelocity.X;
		var currentYVelocity = LinearVelocity.Y;

		// get velocity of the paddle
		var player = GetNode<Player>("/root/Main/Player");
		var playerPaddleVelocity = player.currentVelocity.Y;
		// if ball is moving up and paddle is moving up
		if (currentYVelocity < 0 && playerPaddleVelocity != 0 && playerPaddleVelocity < 0) {
			GD.Print("PADDLE UP & BALL UP");
			currentXVelocity += playerPaddleVelocity;
		}
		// if ball is moving down and paddle is moving down
		if (currentYVelocity > 0 && playerPaddleVelocity != 0 && playerPaddleVelocity > 0) {
			GD.Print("PADDLE DOWN & BALL DOWN");
			currentXVelocity -= playerPaddleVelocity;
		}
		// if ball is moving up and paddle is moving down
		if (currentYVelocity < 0 && playerPaddleVelocity != 0 && playerPaddleVelocity > 0) {
			GD.Print("PADDLE DOWN & BALL UP");
			float dampedVelocity = playerPaddleVelocity;
			currentXVelocity += dampedVelocity;
		}
		// if ball is moving down and paddle is moving up
		if (currentYVelocity > 0 && playerPaddleVelocity != 0 && playerPaddleVelocity < 0) {
			GD.Print("PADDLE UP & BALL DOWN");
			float dampedVelocity = playerPaddleVelocity;
			currentXVelocity -= dampedVelocity;
		}

		// Prevent ball from moving TOO fast OR slow
		currentXVelocity = Mathf.Clamp(currentXVelocity, -600, -275);

		// set the velocity to the opposite X & Y
		// Rotate just a bit to add some variance to exit vector
		LinearVelocity = new Vector2(-currentXVelocity, currentYVelocity)
			.Rotated((float)GD.RandRange(-Mathf.Pi / 6, Mathf.Pi / 6));
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
		player.Hit += OnPlayerHit;
		opponent.Hit += OnOpponentHit;
		topWall.BodyEntered += OnWallBodyEntered;
		bottomWall.BodyEntered += OnWallBodyEntered;
	}

	private void DisconnectSignals()
	{
		var player = GetNode<Player>("/root/Main/Player");
		var opponent = GetNode<Opponent>("/root/Main/Opponent");
		var topWall = GetNode<Area2D>("/root/Main/TopWall");
		var bottomWall = GetNode<Area2D>("/root/Main/BottomWall");
		player.Hit -= OnPlayerHit;
		opponent.Hit -= OnOpponentHit;
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
