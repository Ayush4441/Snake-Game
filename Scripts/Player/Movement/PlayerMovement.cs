using Godot;

public class PlayerMovement : Node2D
{
	[Export]
	public int TileSize = 64;
	[Export]
	public float Speed = 1;
	private Vector2 MoveDirection, MoveAxis;
	private Node2D player;
	
	public void Inputs(Vector2 MoveAxis)
	{
		this.MoveAxis = MoveAxis;
	}

	public override void _Ready()
	{
		player = GetParent().GetParent().GetNode<Node2D>("Player");

		player.Position = player.Position.Snapped(Vector2.One * TileSize);
        player.Position += Vector2.One * TileSize/2;
	}

	public override void _Process(float delta)
	{
		if (MoveDirection != MoveAxis)
		{
			RotatePlayer(delta);
		    MoveDirection = MoveAxis;
		}
		else
			MovePlayer(delta);
	}

	private void MovePlayer(float delta)
	{
		player.Position += MoveDirection * TileSize * delta * Speed;
	}

	private void RotatePlayer(float delta)
	{
		player.Position += MoveAxis * TileSize * delta* Speed;
	}
}
