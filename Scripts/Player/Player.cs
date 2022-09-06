using Godot;
using System;

public class Player : Area2D
{
	private Vector2 MoveAxis;
	private PlayerMovement Movement;
	private const string InputMoveUP = "Player_Move_Up", InputMoveDown = "Player_Move_Down", InputMoveRight = "Player_Move_Right", InputMoveLeft = "Player_Move_Left";
	
	public override void _Ready()
	{
		Movement = GetNode<PlayerMovement>("Player Movement");
	}
	
	public override void _Process(float delta)
	{
		Movement.Inputs(MoveAxis);
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed(InputMoveUP))
		   MoveAxis = Vector2.Up;
		if (inputEvent.IsActionPressed(InputMoveDown))
		   MoveAxis = Vector2.Down;
		if (inputEvent.IsActionPressed(InputMoveRight))
		   MoveAxis = Vector2.Right;
		if (inputEvent.IsActionPressed(InputMoveLeft))
		   MoveAxis = Vector2.Left;
	}
}
