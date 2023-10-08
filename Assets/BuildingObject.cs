using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class BuildingObject : StaticBody3D
{
	public bool spawned = false;
	[Export]
	public int WoodCost;
	[Export]
	public int StoneCost;
	[Export]
	public int IronCost;
	[Export]
	public int GoldCost;
	[Export]
	public int PopulationCost;
	[Export]
	public bool IncreasePopCap = false;
	[Export]
	public int IncreaseCapAmount = 5;

	[Export]
	public bool spawnActor = true;
	[Export]
	public PackedScene actor {get;set;}
	public bool activeBuildableObject = false;
	protected BuildManager buildManager;
	protected GameManager gameManager;
	readonly List<Node> objects = new();

	protected Actor currentActor;

	private Area3D area;

	public override void _Ready()
	{
		gameManager = this.GetGameManager();
		buildManager = this.GetBuildManager();
		area = GetNode<Area3D>("Area3D");
		var callableAreaEntered = new Callable(this, "_on_area_3d_area_entered");
		var callableAreaExited = new Callable(this, "_on_area_3d_area_exited");
		area.Connect("area_entered", callableAreaEntered);
		area.Connect("area_exited", callableAreaExited);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public virtual void runSpawn()
	{
		if (spawnActor)
		{
			var spawnPoint = GetNode<Marker3D>("SpawnPoint");
			currentActor = this.actor.Instantiate<Actor>();
			currentActor.Hut = spawnPoint;
			GetTree().Root.AddChild(currentActor);
			currentActor.GlobalPosition = spawnPoint.GlobalPosition;
			
		}
		if (IncreasePopCap)
		{
			gameManager.MaxPopulation += IncreaseCapAmount;
		}
	}

	public void runDespawn() 
	{
		if(spawnActor){
			currentActor.QueueFree();
		}

		gameManager.Population -= PopulationCost;
		if(IncreasePopCap)
			gameManager.MaxPopulation -= IncreaseCapAmount;
		QueueFree();
	}

	private void _on_area_3d_area_entered(Area3D area)
	{
		if (activeBuildableObject && spawned)
		{
			objects.Add(area);
			buildManager.AbleToBuild = false;
			var objs = objects.Aggregate("",(acc,next) => {
				acc += next.ToString();
				return acc;
			});
			GD.Print(objs);
		}
	}


	private void _on_area_3d_area_exited(Area3D area)
	{
		if (activeBuildableObject && spawned)
		{
			objects.Remove(area);
			var objs = objects.Aggregate("",(acc,next) => {
				acc += next.ToString();
				return acc;
			});
			GD.Print(objs);
			if (!objects.Any())
			{
				buildManager.AbleToBuild = true;
			}
		}
	}

	public void SetDisabled(bool Disabled){
		GetNode<CollisionShape3D>("CollisionShape3D").Disabled = Disabled;
	}
}
