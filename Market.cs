using Godot;
using System;
using System.Security.AccessControl;

public partial class Market : HBoxContainer
{
	GameManager gameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = this.GetGameManager();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("VBoxContainer/HBoxContainer/FoodValue").Text = gameManager.Food.ToString();
		GetNode<Label>("VBoxContainer/HBoxContainer2/IronValue").Text = gameManager.Iron.ToString();
		GetNode<Label>("VBoxContainer2/HBoxContainer/StoneValue").Text = gameManager.Stone.ToString();
		GetNode<Label>("VBoxContainer2/HBoxContainer2/WoodValue").Text = gameManager.Wood.ToString();
	}

	private int buy(int cost, int amount, int item) {
		if(gameManager.Gold >= cost){
			gameManager.Gold -= cost;
			return amount + item;
		}
		return item;
	}


	private int sell(int amount, int money, int item) {
		if(item >= amount){
			gameManager.Gold += money;
			return item - amount;
		}
		return item;
	}

	private void _on_SellFood_button_down()
	{
		gameManager.Food = sell(5, 15, gameManager.Food);
	}

	private void _on_BuyFood_button_down()
	{
		gameManager.Food = buy(25, 5, gameManager.Food);
	}

	private void _on_SellIron_button_down()
	{
		gameManager.Iron = sell(5, 25, gameManager.Iron);
	}

	private void _on_BuyIron_button_down()
	{
		gameManager.Iron = buy(40, 5, gameManager.Iron);
	}

	private void _on_SellStone_button_down()
	{
		gameManager.Stone = sell(5, 25, gameManager.Stone);
	}

	private void _on_BuyStone_button_down()
	{
		gameManager.Stone = buy(40, 5, gameManager.Stone);
	}

	private void _on_SellWood_button_down()
	{
		gameManager.Wood = sell(5, 5, gameManager.Wood);
	}

	private void _on_BuyWood_button_down()
	{
		gameManager.Wood = buy(10, 5, gameManager.Wood);
	}
}
