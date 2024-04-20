using Godot;

public partial class Main : Node
{
	[Export]
    public PackedScene BallScene { get; set; }

	[Export]
	private double _ballSpeedY = 185.0;
	private double _ballSpeedX = 300.0;

	private int _playerScore;
	private int _oppententScore;
	private bool _paused = false;

	private string _latestScorer = "opponent";

	private void OnStartTimerTimeout()
	{
		StartNewRound();
	}

	private void OnRestartGame()
	{
		NewGame();
	}

	private void ResetPlayerPositions()
	{
		// Set position of Player
		var player = GetNode<Player>("Player");
		var playerStartPosition = GetNode<Marker2D>("StartPositionPlayer");
		player.Start(playerStartPosition.Position);

		// Set position of Opponent
		var opponent = GetNode<Opponent>("Opponent");
		var opponentStartPosition = GetNode<Marker2D>("StartPositionOpponent");
		opponent.Start(opponentStartPosition.Position);
	}

	private void StartNewRound()
	{
		ResetPlayerPositions();

		Ball ball = BallScene.Instantiate<Ball>();

		// Connect signal from ball leaving the game area
		ball.OnScore += OnScore;

		GD.Print("New Round Starting!");
		// Reverse direction of "serve" depending on who just scored
		double serveVelocityX = _latestScorer == "player" ? _ballSpeedX * -1 : _ballSpeedX;

		var ballStartPosition = GetNode<Marker2D>("StartPositionBall");
		ball.Place(ballStartPosition.Position);

		ball.LinearVelocity = new Vector2((float)serveVelocityX, (float)GD.RandRange(_ballSpeedY * -1, _ballSpeedY));

		AddChild(ball);
	}

	public void OnScore(string scorer)
	{
		if (scorer == "player")
		{
			_playerScore++;
		} else {
			_oppententScore++;
		}

		_latestScorer = scorer;

		GetNode<HUD>("HUD").ShowRoundOver();

		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_playerScore, "player");
		hud.UpdateScore(_oppententScore, "opponent");

		// start the start timer to start a new round
		var startTimer = GetNode<Timer>("StartTimer");
		// Upon starting a new round we show Round Over for 2 secs
		// And we show Get Ready! for 2 secs.
		// So I update the wait time on the start timer to 4 secs
		startTimer.WaitTime = 4;
		startTimer.Start();
	}

	public void GameOver() {

	}

	public void NewGame() {
		// Set score to 0,0
		_playerScore = 0;
		_oppententScore = 0;

		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_playerScore, "player");
		hud.UpdateScore(_oppententScore, "opponent");
		hud.ShowMessage("Get Ready!");

		// Hide Ready Player Two button
		GetNode<Button>("HUD/PlayerTwoButton").Hide();

		GetNode<AudioStreamPlayer>("Music").Play();

		// Start countdown timer for game start
		var startTimer = GetNode<Timer>("StartTimer");
		startTimer.Start();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("pause") && GetTree().Paused)
		{
			GetTree().Paused = false;
		}
		else if (Input.IsActionJustReleased("pause") && !GetTree().Paused)
		{
			GetTree().Paused = true;
		}
	}
}
