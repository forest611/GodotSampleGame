using Godot;
using System;

public partial class Hud : CanvasLayer
{

	private Label _score;
	private Label _life;
	private Label _message;
	private Button _restartButton;


	[Signal]
	public delegate void RestartEventHandler();

	public override void _Ready()
	{
		_score = GetNode<Label>("Score");
		_life = GetNode<Label>("Life");
		_message = GetNode<Label>("Message");
		_restartButton = GetNode<Button>("Restart");
		_restartButton.Pressed += OnRestart;
		
		_restartButton.Show();
	}

	public void SetMessage(string text)
	{
		_message.Text = text;
	}

	public void SetScore(int score)
	{
		_score.Text = "Score: " + score;
	}

	public void SetLife(int life)
	{
		_life.Text = "Life: " + life;
	}

	public void GameOver(int score)
	{
		SetMessage("Game Over!\nScore: " + score);
		SetLife(3);
		SetScore(0);
		_restartButton.Show();
	}
	
	private void OnRestart()
	{
		_restartButton.Hide();
		EmitSignal(SignalName.Restart);
	}



}
