using Godot;
using System;

public partial class Main : Node
{

	[Export]
	public PackedScene Mob;
	[Export]
	public int Life = 3;
	[Export]
	public int Countdown = 3;

	private int _score;
	private int _life;
	private int _timerSecond;
	private GameStatus _gameStatus = GameStatus.Ready;

	private Timer _timer;
	private Player _player;
	private Hud _hud;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_player = GetNode<Player>("Player");
		_hud = GetNode<Hud>("HUD");
		_player.Hit += OnPlayerHit;
		_timer.Timeout += OnTimerTimeout;
		_hud.Restart += NewGame;

	}

	public void GameOver()
	{
		_timer.Stop();
		_player.Hide();
		_hud.GameOver(_score);
	}

	public void NewGame()
	{
		_score = 0;
		_life = Life;
		_timerSecond = Countdown;
		_gameStatus = GameStatus.Ready;

		_player.Start(GetNode<Marker2D>("Marker2D").Position);
		RemoveAllMobs();
		_timer.Start();
		_hud.SetScore(_score);
		_hud.SetLife(Life);
		_hud.SetMessage($"{_timerSecond}");

	}

	private void OnTimerTimeout()
	{
		_timerSecond -= 1;
		
		if (_gameStatus == GameStatus.Ready)
		{

			if (_timerSecond > 0)
			{
				_hud.SetMessage($"{_timerSecond}");
			}
			else
			{
				_hud.SetMessage("GO!");
				_gameStatus = GameStatus.Play;
			}			
			return;
		}

		_hud.SetScore(_score);
		_hud.SetLife(_life);
		_hud.SetMessage("");

		if (_gameStatus == GameStatus.Play)
		{
			MoveMob();
			_score += 1;
		}
	}

	private void MoveMob()
	{
		// Create a new instance of the Mob scene.
		Mob mob = Mob.Instantiate<Mob>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	private void OnPlayerHit(Player player)
	{
		if (player._isDamaging) return;

		_life -= 1;
		if (_life <= 0)
		{
			GameOver();
			return;
		}

		player.Damage();
	}

	// Mainクラス内で、子ノードとして追加されたすべてのMobを削除
	private void RemoveAllMobs()
	{
		foreach (var child in GetChildren())
		{
			if (child is Mob mob) mob.QueueFree();
		}
	}

	enum GameStatus {
		Ready,
		Play,
		GameOver
	}
}
