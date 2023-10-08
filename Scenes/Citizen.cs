using Godot;
using System;

public partial class Citizen : CharacterBody3D
{
	
	
	public Task currenTask = Task.Walking;
	public Marker3D FirePitPos;
	public NavigationAgent3D navagent;
	public enum Task
	{
		Walking,
		Sitting	
	}

	[Export]
	int walkSpeed = 6;
	
	GameManager gameManager;
	BuildManager buildManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void SpawnObjectSetup(){
		navagent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navagent.TargetPosition = FirePitPos.GlobalPosition;
		gameManager = this.GetGameManager();
		buildManager = this.GetBuildManager();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(currenTask){
			case Task.Sitting:
				break;
			case Task.Walking:
			if (navagent.IsNavigationFinished())
				{
					currenTask = Task.Sitting;
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
	}
}
