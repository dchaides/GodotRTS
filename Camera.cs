using Godot;
using System;

public partial class Camera : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		var mousePos = GetViewport().GetMousePosition();
		if (mousePos.X < 10)
		{
			GlobalTranslate(-GlobalTransform.Basis.X);
		}
		else if (mousePos.X > viewportSize.X - 10)
		{
			GlobalTranslate(GlobalTransform.Basis.X);
		}
		if (mousePos.Y < 10)
		{
			GlobalTranslate(-GlobalTransform.Basis.Z);
		}
		else if (mousePos.Y > viewportSize.Y - 10)
		{
			GlobalTranslate(GlobalTransform.Basis.Z);
		}
		var cam = GetNode<Camera3D>("Camera3D");
		if(Input.IsActionJustReleased("MiddleMouseButton")){
			GlobalRotate(new Vector3(0, 1, 0),  Mathf.DegToRad(90));
		}
		if(Input.IsActionJustReleased("MouseWheelUp")){
			if(cam.GlobalPosition.DistanceTo(GlobalPosition) > 10)
			cam.GlobalTranslate(-cam.GlobalTransform.Basis.Z * 2 );
		}
		if(Input.IsActionJustReleased("MouseWheelDown")){
			if(cam.GlobalPosition.DistanceTo(GlobalPosition) < 50)
			cam.GlobalTranslate(cam.GlobalTransform.Basis.Z * 2 );
		}
	}
}
