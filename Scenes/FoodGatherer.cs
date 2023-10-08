using Godot;
using System;
using System.Linq;

public partial class FoodGatherer : Actor
{

	[Export]
	public int resourceGenerationAmount;

	public int foodIndex;
	public Node3D[] foodProducers;

	[Export]
	public float walkSpeed = 6f;
	
	public enum Task
	{
		GettingFood,
		Searching,
		Delivering,
		Walking
	}
	Task currentTask = Task.Searching;
	
	int HeldResourceAmount = 0;
	NavigationAgent3D navagent;
	bool runOnce = true;
	private GameManager gameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		navagent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		gameManager = this.GetGameManager();
		foodProducers = Hut.GetNode("Resources").GetChildren().Cast<Node3D>().ToArray();
	}

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		switch (currentTask)
		{
			case Task.GettingFood:
				if (runOnce)
				{
					runOnce = false;
					var timer = GetTree().CreateTimer(2.0d);
					await ToSignal(timer, "timeout");
					runOnce = true;
					HeldResourceAmount += resourceGenerationAmount;
					currentTask = Task.Searching;
					if(foodIndex >= foodProducers.Length) {
						currentTask = Task.Delivering;
					}
					
				}
				break;
			case Task.Searching:
				if (foodProducers.Any())
				{
					navagent.TargetPosition = foodProducers[foodIndex].GlobalPosition;
				}
				currentTask = Task.Walking;
				break;
			case Task.Delivering:
				var granaries = GetTree().GetNodesInGroup("Granery").Cast<BuildingObject>().Where(b => b.spawned).ToArray();
				if (granaries.Any())
				{
					var nearestGranary = granaries.OrderBy(r => r.GlobalPosition.DistanceTo(GlobalPosition)).First();
					navagent.TargetPosition = nearestGranary.GetNode<Node3D>("SpawnPoint").GlobalPosition;
				}
				currentTask = Task.Walking;
				break;
			case Task.Walking:
				if (navagent.IsNavigationFinished())
				{
					if (foodIndex  != foodProducers.Length)
					{
						currentTask = Task.GettingFood;
						foodIndex += 1;
					}
					else
					{
						gameManager.Food += HeldResourceAmount;
						foodIndex = 0;
						HeldResourceAmount = 0;
						currentTask = Task.Searching;
					}
					return;
				}
				var TargetPosition = navagent.GetNextPathPosition();
				var direction = GlobalPosition.DirectionTo(TargetPosition);
				Velocity = direction * walkSpeed;
				MoveAndSlide();
				break;
			default:
				break;
		}
		GetNode<Label3D>("Label3D").Text = currentTask.ToString();
	}
}
