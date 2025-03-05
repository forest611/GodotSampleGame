using Godot;
using System;

public partial class Player : Area2D
{
	
	[Export]
	public int Speed { get; set; } = 4;
	[Signal]
	public delegate void HitEventHandler(Player player);

	public Vector2 ScreenSize;
	private Timer _timer;
	public bool _isDamaging = false;

	// クラスのフィールドとして追加
	private Vector2 _touchDirection = Vector2.Zero;
	private bool _isTouching = false;

	// _Inputメソッドを追加
	public override void _Input(InputEvent @event)
	{
		// タッチ開始
		if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
		{
			_isTouching = true;
			_touchDirection = Vector2.Zero;
		}
		// タッチ終了
		else if (@event is InputEventScreenTouch touchEvent2 && !touchEvent2.Pressed)
		{
			_isTouching = false;
			_touchDirection = Vector2.Zero;
		}
		// ドラッグ
		else if (@event is InputEventScreenDrag dragEvent)
		{
			// ドラッグベクトルを計算
			Vector2 dragVector = dragEvent.Relative;
			
			// 一定距離以上ドラッグした場合のみ方向を設定
			if (dragVector.Length() > 5)
			{
				_touchDirection = dragVector.Normalized();
			}
		}
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnDamage;
		_timer.Start();
		BodyEntered += OnPlayerHit;
	}

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}
	
	public override void _Process(double delta)
	{
		Move(delta);
	}

	private void Move(double delta)
	{

		var velocity = GetPCInput();
		
		if (_touchDirection != Vector2.Zero)
		{
			velocity = _touchDirection;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}		

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about the following boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
	}

	private Vector2 GetPCInput()
	{
		var velocity = Vector2.Zero; // The player's movement vector.
		
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X = 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X = -1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y = 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y = -1;
		}

		return velocity;
	}

	private void OnDamage()
	{
		if (!_isDamaging)
		{
			Show();
			return;
		} 

		if (Visible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public void Death()
	{
		_isDamaging = true;
		_timer.Stop();
		Hide();

	}

	public void Damage()
	{
		_isDamaging = true;
		GetTree().CreateTimer(2.0).Timeout += () => _isDamaging = false;
	}

	private void OnPlayerHit(Node2D body)
	{
		EmitSignal(SignalName.Hit,this);
	}

}
