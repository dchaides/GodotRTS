using Godot;
using System;

public partial class BuildManager : Node3D
{
	public bool AbleToBuild = true;
	GameManager gameManager;
	PackedScene WoodCuttersHut = ResourceLoader.Load<PackedScene>("res://Assets/WoodCutters.tscn");
	PackedScene IronWorks = ResourceLoader.Load<PackedScene>("res://Assets/IronSmelter.tscn");
	PackedScene StoneCuttersHut = ResourceLoader.Load<PackedScene>("res://Assets/StoneMasons.tscn");
	PackedScene Granary = ResourceLoader.Load<PackedScene>("res://Assets/Granery.tscn");
	PackedScene Stockpile = ResourceLoader.Load<PackedScene>("res://Assets/Stockpile.tscn");
	PackedScene House = ResourceLoader.Load<PackedScene>("res://Assets/House.tscn");
	PackedScene Wall = ResourceLoader.Load<PackedScene>("res://Assets/wallNarrow.tscn");
	PackedScene CornerWall = ResourceLoader.Load<PackedScene>("res://Assets/wallNarrowCorner.tscn");
	PackedScene Gate = ResourceLoader.Load<PackedScene>("res://Assets/wallNarrowGate.tscn");
	PackedScene Orchard = ResourceLoader.Load<PackedScene>("res://Assets/Orchard.tscn");
	BuildingObject currentSpawnable;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = (GameManager)GetNode("/root/GameManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _Process(double delta)
	{
		if(gameManager.CurrentState == State.Buiding) {
			var viewport = GetViewport();
			var camera = viewport.GetCamera3D();
			var from = camera.ProjectRayOrigin(viewport.GetMousePosition());
			var to = from + camera.ProjectRayNormal(viewport.GetMousePosition()) * 1000;
			var cursorPos = new Plane(Vector3.Up, Transform.Origin.Y).IntersectsRay(from, to);
			if(cursorPos.HasValue) {
				currentSpawnable.GlobalPosition = new Vector3(Mathf.Round(cursorPos.Value.X), cursorPos.Value.Y, Mathf.Round(cursorPos.Value.Z));
				currentSpawnable.activeBuildableObject = true;
				if (AbleToBuild && CanAfford(currentSpawnable) && (gameManager.AlvPopulation >= currentSpawnable.PopulationCost) )
				{
					if (Input.IsActionJustReleased("LeftMouseDown"))
					{
						var obj = currentSpawnable.Duplicate() as BuildingObject;
						var navRegion = GetTree().GetNodesInGroup("NavRegion")[0] as NavigationRegion3D;
						navRegion.AddChild(obj);
						obj.activeBuildableObject = false;
						obj.runSpawn();
						gameManager.RemoveCitizen(obj.PopulationCost);
						obj.spawned =  true;
						chargeObj(currentSpawnable);
						obj.SetDisabled(false);
						obj.GlobalPosition = currentSpawnable.GlobalPosition;
						navRegion.BakeNavigationMesh();
					}
				}

				if(Input.IsActionJustReleased("RightMouseButtonDown")){
					currentSpawnable.QueueFree();
					currentSpawnable = null;
					gameManager.CurrentState = State.Play;
				}

				if(Input.IsActionJustReleased("RotateRightButton")){
					currentSpawnable.GlobalRotate(new Vector3(0, 1, 0),  Mathf.DegToRad(90));
				}

				if(Input.IsActionJustReleased("RotateLeftButton")){
					currentSpawnable.GlobalRotate(new Vector3(0, 1, 0),  Mathf.DegToRad(-90));
				}
			} 
		}

		if(gameManager.CurrentState == State.Destroying){
			if(currentSpawnable !=null && IsInstanceIdValid(currentSpawnable.GetInstanceId())) {
				currentSpawnable.QueueFree();
				currentSpawnable = null;
			}
			if (Input.IsActionJustReleased("LeftMouseDown"))
			{
				var viewport = GetViewport();
				var camera = viewport.GetCamera3D();
				var from = camera.ProjectRayOrigin(viewport.GetMousePosition());
				var to = from + camera.ProjectRayNormal(viewport.GetMousePosition()) * 1000;
				var space_state = GetWorld3D().DirectSpaceState;
                var parameters = new PhysicsRayQueryParameters3D
                {
                    From = from,
                    To = to
                };
                var result = space_state.IntersectRay(parameters);
				if(result.TryGetValue("collider", out var variant )) {
					if(variant.As<Node3D>().IsInGroup("building")){
						variant.As<BuildingObject>().runDespawn();
					}
				}
			}
		}
	}

	public bool CanAfford(BuildingObject obj) => 
		(gameManager.Wood - obj.WoodCost) >= 0 && 
		(gameManager.Stone - obj.StoneCost) >= 0 && 
		(gameManager.Gold - obj.GoldCost) >= 0 && 
		(gameManager.Iron - obj.IronCost) >= 0;

	public void chargeObj(BuildingObject obj) {
		gameManager.Wood -= obj.WoodCost;
		gameManager.Iron -= obj.StoneCost;
		gameManager.Stone -= obj.StoneCost;
		gameManager.Gold -= obj.GoldCost;
	}

	public void SpawnStockpile() {
		SpawnObj(Stockpile);
	}

	public void SpawnStonecutter() {
		SpawnObj(StoneCuttersHut);
	}

	public void SpawnWoodcutter() {
		SpawnObj(WoodCuttersHut);
	}

	public void SpawnIronSmelter() {
		SpawnObj(IronWorks);
	}

	public void SpawnGranery() {
		SpawnObj(Granary);
	}

	public void SpawnOrchard() {
		SpawnObj(Orchard);
	}

	public void SpawnHouse() {
		SpawnObj(House);
	}

	public void SpawnWallNarrow() {
		SpawnObj(Wall);
	}

	public void SpawnWallCorner() {
		SpawnObj(CornerWall);
	}

	public void SpawnGate() {
		SpawnObj(Gate);
	}

	private void SpawnObj(PackedScene obj){
		currentSpawnable?.QueueFree();
		currentSpawnable = obj.Instantiate<BuildingObject>();
		currentSpawnable.SetDisabled(true);
		GetTree().Root.AddChild(currentSpawnable);
		gameManager.CurrentState = State.Buiding;
	}
}
