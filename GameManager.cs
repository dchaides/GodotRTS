using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum State {
	Play,
	Buiding,
	Destroying
}

public partial class GameManager : Node
{
	public State CurrentState = State.Play;
	public int Wood = 30;
	public int Stone = 20;
	public int Gold = 100;
	public int Iron = 10;
	public int Food = 1000;
	public int Population = 0;
	public int MaxPopulation = 4;
	public int AlvPopulation = 0;
	public PackedScene Citizen;
	public int Happiness = 1;
	public bool foodBool = true;

	public float taxRate = 2;
	bool SpawnReady = true;
	List<Marker3D> firePitSpaces; 
	List<Marker3D> occupiedFirePitSpaces = new List<Marker3D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Citizen = ResourceLoader.Load<PackedScene>("res://Scenes/Citizen.tscn");
		firePitSpaces = GetTree().GetNodesInGroup("CitizenSpawnPoint").Cast<Marker3D>().ToList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if( (Population < MaxPopulation) && SpawnReady && (Happiness > 0) &&  (firePitSpaces.Count() > 0)) {
			SpawnReady = false;
			var timer = GetTree().CreateTimer(3.0f);
			await ToSignal(timer, "timeout");
			SpawnReady = true;
			var citizen = Citizen.Instantiate<Citizen>();
			firePitSpaces[0].AddChild(citizen);
			citizen.FirePitPos = firePitSpaces.First();
			citizen.SpawnObjectSetup();
			occupiedFirePitSpaces.Add(firePitSpaces.First());
			firePitSpaces.RemoveAt(0);
			Population += 1;
			AlvPopulation += 1;

		} else if (SpawnReady && (Happiness < 0)){
			SpawnReady = false;
			var timer = GetTree().CreateTimer(3.0f);
			await ToSignal(timer, "timeout");
			SpawnReady = true;
			if(AlvPopulation > 0) {
				RemoveCitizen(1);
			}
		}

		if(foodBool) {
			foodBool = false;
			var timer = GetTree().CreateTimer(6.0f);
			await ToSignal(timer, "timeout");
			Food -= Population;
			if(Food< 0) {
				Food = 0;
			}
			foodBool = true;
			Gold += Mathf.RoundToInt(Population * taxRate);
			var happinessValue = 1;


			if (Food > 0) {
				happinessValue += 1;
			} else {
				happinessValue -= 10;
			}

			if (taxRate > 0) {
				happinessValue -= Mathf.RoundToInt(taxRate / 2);
			}
			if(taxRate < 0) {
				happinessValue += Math.Abs(Mathf.RoundToInt(taxRate / 2));
			}

			Happiness += happinessValue;
			if(Happiness >= 2) { 
				Happiness = 2;
			}
			else if (Happiness <= -2) {
				Happiness = -2;
			}
		}
	}

    public void RemoveCitizen(int cost)
    {
		for (var i = 0; i < cost; i++)
		{
			firePitSpaces.Add(occupiedFirePitSpaces[0]);
			var temp = occupiedFirePitSpaces[0];
			deleteChildren(temp);
			occupiedFirePitSpaces.RemoveAt(0);
			AlvPopulation -= 1;
			Population -= 1;
		}
	}

    private void deleteChildren(Marker3D node)
    {
        foreach(var child in node.GetChildren()) {
			node.RemoveChild(child);
			child.QueueFree();
		}
    }
}
