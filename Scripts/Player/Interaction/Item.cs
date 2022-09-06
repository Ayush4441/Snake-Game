using Godot;
using System;

public class Item : Node2D
{
    private CollisionShape2D Collider;

    public override void _Ready()
    {
        Collider = GetNode<CollisionShape2D>("Collider");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
