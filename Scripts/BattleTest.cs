using Godot;
using System.Threading.Tasks;

public partial class BattleTest : Node2D
{
	// Declaring our enemies and player 
	private int playerHp = 30;
	private int enemyHp = 20;
	private bool playerTurn = true;
	private bool battleOver = false;

	// Setting up labels inputs
	private Label playerHpLabel;
	private Label enemyHpLabel;
	private Label messageLabel;

	private Button attackButton;
	private Button defendButton;
	private Button dodgeButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("BattleTest ready function started");
		playerHpLabel = GetNode<Label>("CanvasLayer/BattleUI/PlayerHPLabel");
		enemyHpLabel = GetNode<Label>("CanvasLayer/BattleUI/EnemyHPLabel");
		messageLabel = GetNode<Label>("CanvasLayer/BattleUI/MessageLabel");

		attackButton = GetNode<Button>("CanvasLayer/BattleUI/ActionPanel/VBoxContainer/AttackButton");
		defendButton = GetNode<Button>("CanvasLayer/BattleUI/ActionPanel/VBoxContainer/DefendButton");
		dodgeButton = GetNode<Button>("CanvasLayer/BattleUI/ActionPanel/VBoxContainer/DodgeButton");

		GD.Print("All UI Nodes loaded");

		UpdateUI();
		messageLabel.Text = "Battle started!";

		attackButton.Pressed += OnAttackPressed;
		defendButton.Pressed += OnDefendPressed;
		dodgeButton.Pressed += OnDodgePressed;

	}
	private void UpdateUI()
	{
		playerHpLabel.Text = $"Player HP: {playerHp}";
		enemyHpLabel.Text = $"Player HP: {enemyHp}";

		bool buttonsEnabled = playerTurn && !battleOver;
		attackButton.Disabled = !buttonsEnabled;
		defendButton.Disabled = !buttonsEnabled;
		dodgeButton.Disabled = !buttonsEnabled;
	}

	private async void OnAttackPressed()
	{
		if (!playerTurn || battleOver)
		{
			return;
		}

		GD.Print("Attack button pressed");
		playerTurn = false;

		int damage = 5;
		enemyHp -= damage;
		if (enemyHp < 0)
		{
			enemyHp = 0;
		}
		messageLabel.Text = $"You attacked for {damage} damage!";
		UpdateUI();

		if (enemyHp <= 0)
		{
			EndBattle(true);
			return;
		}

		await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
		await EnemyTurn();
	}

	private async void OnDefendPressed()
	{
		if (!playerTurn || battleOver)
		{
			return;
		}
		
		GD.Print("Defend button pressed");
		
		playerTurn = false;
		messageLabel.Text = $"Preparing your defenses!";
		UpdateUI();
		
		await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
		await EnemyTurn();
	}

	private async void OnDodgePressed()
	{
		if (!playerTurn || battleOver)
		{
			return;
		}
		GD.Print("Dodge button pressed");

		playerTurn = false;

		bool dodgeSuccess = GD.Randf() < 0.5f;

		if (dodgeSuccess)
		{
			enemyHp -= 1;
			
			if (enemyHp < 0)
			{
				enemyHp = 0;
			}

			messageLabel.Text = $"Dodge Landed! Enemy lost 1 hp....";
		} 
		else 
		{
			messageLabel.Text = $"Your dodge missed!";
		}


		UpdateUI();

		if (enemyHp <= 0)
		{
			EndBattle(true);
			return;
		}

		await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
		await EnemyTurn();
	}



	private async Task EnemyTurn(bool defending = false)
	{
		if (battleOver)
		{
			return;
		}
			

		GD.Print("Enemy turn started");

		int damage =  defending ?2 : 4;
		playerHp -= damage;

		if (playerHp < 0)
		{
			playerHp = 0;
		}
			
		if (defending)
		{
			messageLabel.Text = $"You defended! Enemy attacked for {damage} damage.";
		}
		else
		{
			messageLabel.Text = $"Enemy attacked for {damage} damage!";
		}

		messageLabel.Text = $"Enemy attacked for {damage} damage!";
		UpdateUI();

		if (playerHp <= 0)
		{
			EndBattle(false);
			return;
		}

		await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);

		playerTurn = true;
		messageLabel.Text = "Choose your next action.";
		UpdateUI();

		GD.Print("Enemy turn ended");
	}
	

	private void EndBattle(bool playerWon)
	{
		battleOver = true;
		UpdateUI();

		messageLabel.Text = playerWon 
			? "You won the battle!"
			: "You lost the battle!";
	}

}
