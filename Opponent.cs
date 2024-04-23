using System;
using Godot;

public partial class Opponent : Area2D
{
	[Signal]
	public delegate void HitEventHandler(string body);

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

	public class Prediction 
	{
		public float DX {get;set;} = 0;
		public float DY {get;set;} = 0;
		public int Since {get;set;} = 0;
	}

	public struct Level {
		public double AiReaction {get;set;}
		public int AiError {get;set;}

		public Level(double aiReaction, int aiError)
		{
			AiReaction = aiReaction;
			AiError = aiError;
		}
	}

	Level[] levels = new Level[]
	{
		new Level(0.2, 40),
		new Level(0.3, 50),
		new Level(0.4, 60),
		new Level(0.5, 70),
		new Level(0.6, 80),
		new Level(0.7, 90),
		new Level(0.8, 100),
		new Level(0.9, 110),
		new Level(1.0, 120),
		new Level(1.1, 130),
		new Level(1.2, 140),
		new Level(1.3, 150),
		new Level(1.4, 160),
		new Level(1.5, 170),
		new Level(1.6, 180),
		new Level(1.7, 190),
		new Level(1.8, 200)
	};

	Prediction prediction = new Prediction();

	public void Start(Vector2 position)
	{
		Position = position;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.Hit, "opponent");
	}

	private void OnHudEnablePlayerTwo()
	{
		readyPlayerTwo = true;
	}

	private void MoveUp(ref Vector2 velocity)
	{
		velocity.Y -= 1;
	}

	private void MoveDown(ref Vector2 velocity)
	{
		velocity.Y += 1;
	}

	private void StopMoving(ref Vector2 velocity)
	{
		velocity.Y = 0;
	}

	private void ControlPlayer(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_up_player_2"))
		{
			MoveUp(ref velocity);
		}

		if (Input.IsActionPressed("move_down_player_2"))
		{
			MoveDown(ref velocity);
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

	private void Predict(double delta, ref RigidBody2D ball)
	{
		// Only predict when ball has changed directions or passed a timeout
		if ((prediction.DX * ball.LinearVelocity.X > 0) &&
			(prediction.DY * ball.LinearVelocity.Y > 0) &&
			(prediction.Since < levels[0].AiError))
			{
				GD.Print("What is delta for Since: ", delta);
				prediction.Since += (int)delta;
				return;
			}
		
		// Determine ball interception point
		
	}

	private void NewAIControl(double delta)
	{
		var ball = GetNodeOrNull<RigidBody2D>("/root/Main/Ball");

		if(ball != null)
		{
			Vector2 velocity = Vector2.Zero;

			if(ball.LinearVelocity.X < 0)
			{
				StopMoving(ref velocity);
				return;
			}

			Predict(delta, ref ball);
		}
	}

	// Good reference to AI using a Prediction class
	// https://github.com/jakesgordon/javascript-pong/blob/master/part5/pong.js#L328
	// private void AIControl(double delta)
	// {
	// 	var ball = GetNodeOrNull<RigidBody2D>("/root/Main/Ball");

	// 	if (ball != null)
	// 	{
	// 		// Move paddle to center if the ball is moving away
	// 		// if (ball.LinearVelocity.X < 0) {
	// 		// 	return;
	// 		// }

	// 		float windowSize = 32;
	// 		float ballYPosition = ball.Position.Y;

	// 		var lower = ballYPosition - windowSize;
	// 		var upper = ballYPosition + windowSize;

	// 		Vector2 window = new Vector2((float)lower, (float)upper);

	// 		// on startup set _ballPosition once
	// 		if (_ballPosition == 0)
	// 		{
	// 			_ballPosition = ballYPosition;
	// 		}

	// 		if (upDirection && _ballPosition > window.X) 
	// 		{
	// 			_ballPosition -= 1;
	// 		} 
	// 		else if (!upDirection && _ballPosition < window.Y)
	// 		{	
	// 			_ballPosition += 1;
	// 		}
	// 		else 
	// 		{
    //             upDirection = !upDirection;
	// 		}

	// 		// Get difference between ball and drift
	// 		adjustedBallPosition = (ballYPosition - _ballPosition) * difficulty;

	// 		float finalBallPosition = ballYPosition + adjustedBallPosition;

	// 		Position = new Vector2(
	// 			x: Mathf.Clamp(Position.X, windowSize, ScreenSize.X - (float)windowSize),
	// 			y: Mathf.Clamp(finalBallPosition, windowSize, ScreenSize.Y - (float)windowSize )
	// 		);
	// 	}
	// }

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
			NewAIControl(delta);
			// AIControl(delta);
		}

	}
}
