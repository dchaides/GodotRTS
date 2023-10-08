using Godot;
using System;

public partial class InterfaceManager : Control
{
	BuildManager buildManager;
	GameManager gameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildManager = this.GetBuildManager();
		gameManager = this.GetGameManager();


		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		GetNode<Label>("Control/VBoxContainer/Wood/WoodValue").Text = gameManager.Wood.ToString();
		GetNode<Label>("Control/VBoxContainer/Stone/StoneValue").Text = gameManager.Stone.ToString();
		GetNode<Label>("Control/VBoxContainer/Iron/IronValue").Text = gameManager.Iron.ToString();
		GetNode<Label>("Control/VBoxContainer/Food/FoodValue").Text = gameManager.Food.ToString();
		GetNode<Label>("Control/VBoxContainer/Gold/GoldValue").Text = gameManager.Gold.ToString();
		GetNode<Label>("Control/VBoxContainer2/Pop/PopValue").Text = $"{gameManager.AlvPopulation}/{gameManager.MaxPopulation}";
		GetNode<Label>("Control/VBoxContainer2/Hap/HapValue").Text = gameManager.Happiness.ToString();
		GetNode<Label>("TabContainer/Economy/Control/TaxRate").Text = gameManager.taxRate.ToString() + " %";
	}

	private void _on_build_stockpile_button_button_down() { 
		buildManager.SpawnStockpile();
	}
	
	private void _on_build_wood_cutter_button_button_down() { 
		buildManager.SpawnWoodcutter();
	}

	private void _on_build_stone_cutter_button_button_down() { 
		buildManager.SpawnStonecutter();
	}

	private void _on_build_iron_smelter_button_button_down() { 
		buildManager.SpawnIronSmelter();
	}

	private void _on_build_Granery_button_button_down() { 
		buildManager.SpawnGranery();
	}

	private void _on_build_orchard_button_button_down() { 
		buildManager.SpawnOrchard();
	}

	private void _on_build_house_button_button_down() { 
		buildManager.SpawnHouse();
	}

	private void _on_build_wall_button_button_down() { 
		buildManager.SpawnWallNarrow();
	}

	private void _on_build_wall_corner_button_button_down() { 
		buildManager.SpawnWallCorner();
	}

	private void _on_build_gate_button_button_down() { 
		buildManager.SpawnGate();
	}

	private void _on_area_2d_area_entered(Area2D area) {
		buildManager.AbleToBuild = false;
	}

	private void _on_area_2d_area_exited(Area2D area){
		buildManager.AbleToBuild = true;
	}

	private void _on_DestroyMode_button_down() {
		gameManager.CurrentState = State.Destroying;

	}

	private void _on_IncreaseTaxes_button_down(){
		gameManager.taxRate += 2;
	}

	private void _on_DecreaseTaxes_button_down(){
		gameManager.taxRate -= 2;
	}
}
