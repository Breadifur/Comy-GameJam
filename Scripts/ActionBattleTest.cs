using Godot;
using System;
using System.Threading.Tasks;

public partial class ActionBattleTest : Node2D
{
	private int playerHp = 30;
	private int enemyHp = 20;
	private bool canAttack = true;
	private bool battleOver = false;

	private Node2D player;
	private Node2D enemy;
	private Area2D enemyArea;

	private Label playerHpLabel;
	private Label enemyHpLabel;
	private Label messageLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Node2D>("Player");
		enemy = GetNode<Node2D>("Enemy");
		enemyArea = GetNode<Area2D>("Enemy/Area2D");

		playerHpLabel = GetNode<Label>("CanvasLayer/BattleUI/PlayerHPLabel");
		enemyHpLabel = GetNode<Label>("CanvasLayer/BattleUI/EnemyHPLabel");
		messageLabel = GetNode<Label>("CanvasLayer/BattleUI/MessageLabel");

		UpdateUI();

		messageLabel.Text = "Move closer and press attack!";

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		
		if(battleOver || !canAttack)
		{
			return;
		}

		if (Input.IsActionJustPressed("attack"))
		{
			await TryAttack();
		}
	}

	private async Task TryAttack()
	{
		GD.Print("Attack button pressed");

		if (battleOver)
		{
			return;
		}

		canAttack = false;

		float distanceToEnemy = player.GlobalPosition.DistanceTo(enemy.GlobalPosition);

		if (distanceToEnemy <= 90f)
		{
			int damage = 5;
			enemyHp -= damage;

			if (enemyHp < 0)
			{
				enemyHp = 0;
			}

			messageLabel.Text = $"Hit! Enemy took {damage} damage.";
			UpdateUI();

			await TryRangeAttack();

			if (enemyHp <= 0)
			{
				EndBattle(true);
				return;
			}
		}
		else
		{
			messageLabel.Text = "Enemy too far away to attack...";
		}

		await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
		canAttack = true;
	}

	private async Task TryRangeAttack()
	{
		if(enemy is CanvasItem enemyCanvasItem)
		{
			enemyCanvasItem.Modulate = new Color(1, 0.5f, 0.5f);
			await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
			enemyCanvasItem.Modulate = Colors.White;
		}
	}

	private void UpdateUI()
	{
		playerHpLabel.Text = $"Player HP: {playerHp}";
		enemyHpLabel.Text = $"Enemy HP: {enemyHp}";
	}
	private void EndBattle(bool playerWon)
	{
		battleOver = true;
		messageLabel.Text = playerWon
			? "You defeated da ENEMY!"
			: "You have been defeared!";
	}
}
