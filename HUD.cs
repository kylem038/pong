using Godot;

public partial class HUD : CanvasLayer
{
	[Signal]
    public delegate void StartGameEventHandler();

	[Signal]
	public delegate void EnablePlayerTwoEventHandler();

	[Signal]
	public delegate void RestartGameEventHandler();

	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowRoundOver()
	{
		ShowMessage("Round Over");

		var roundOverMessageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(roundOverMessageTimer, Timer.SignalName.Timeout);

		// TODO: This shouldnt trigger round start until after start timer finishes
		ShowMessage("Get Ready!");

		var getReadyMessageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(getReadyMessageTimer, Timer.SignalName.Timeout);

			
		var message = GetNode<Label>("Message");
		message.Hide();
	}



	public void UpdateScore(int score, string scorer)
	{
		if (scorer == "player")
		{
			GetNode<Label>("PlayerScore").Text = score.ToString();
		} 
		else 
		{
			GetNode<Label>("OpponentScore").Text = score.ToString();
		}
	}

	private void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	public void OnPlayerTwoPressed()
	{
		GetNode<Button>("PlayerTwoButton").Hide();
		EmitSignal(SignalName.EnablePlayerTwo);
	}

	public void OnRestartButtonPressed()
	{
		GetNode<Button>("RestartButton").Hide();
		EmitSignal(SignalName.RestartGame);
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
