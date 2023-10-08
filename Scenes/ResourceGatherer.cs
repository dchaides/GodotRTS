using Godot;
using System;
using System.Linq;

public partial class ResourceGatherer : Actor
{

	[Export]
	public int resourceGenerationAmount;
	public enum Resource {
		stone,
		iron,
		wood,
		gold

	}

	[Export]
	public Resource resourceToGet;
	[Export]
	public float walkSpeed = 6f;
	
	public enum Task
	{
		GettingResources,
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

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		switch (currentTask)
		{
			case Task.GettingResources:
				if (runOnce)
				{
					runOnce = false;
					var timer = GetTree().CreateTimer(2.0d);
					await ToSignal(timer, "timeout");
					runOnce = true;
					HeldResourceAmount = resourceGenerationAmount;
					currentTask = Task.Delivering;
				}
				break;
			case Task.Searching:
				var resources = GetTree().GetNodesInGroup(resourceToGet.ToString()).Cast<Node3D>().ToArray();
				if (resources.Any())
				{
					var nearestResourceObject = resources.OrderBy(r => r.GlobalPosition.DistanceTo(GlobalPosition)).First();
					navagent.TargetPosition = nearestResourceObject.GlobalPosition;
				}
				currentTask = Task.Walking;
				break;
			case Task.Delivering:
				var stockpiles = GetTree().GetNodesInGroup("Stockpile").Cast<BuildingObject>().Where(b => b.spawned).ToArray();
				if (stockpiles.Any())
				{
					var nearestStockpile = stockpiles.OrderBy(r => r.GlobalPosition.DistanceTo(GlobalPosition)).First();
					navagent.TargetPosition = nearestStockpile.GetNode<Node3D>("SpawnPoint").GlobalPosition;
				}
				currentTask = Task.Walking;
				break;
			case Task.Walking:
				if (navagent.IsNavigationFinished())
				{
					if (HeldResourceAmount == 0)
					{
						currentTask = Task.GettingResources;
					}
					else
					{
						switch (resourceToGet.ToString())
						{
							case "iron":
								gameManager.Iron += HeldResourceAmount;
								break;
							case "wood":
								gameManager.Wood += HeldResourceAmount;
								break;
							case "gold":
								gameManager.Gold += HeldResourceAmount;
								break;
							case "stone":
								gameManager.Stone += HeldResourceAmount;
								break;
							default:
								break;
						}
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
