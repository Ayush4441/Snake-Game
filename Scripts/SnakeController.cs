using System;
using System.Collections.Generic;
using Godot;

public class SnakeController : Node
{
    public enum FaceTowards
    {
        Up,
        Down,
        Right,
        Left
    }
    private TileMap Map;
    //Snake Parts
    private Vector2 SnakeHead = new Vector2(2, 1), SnakeTail = new Vector2(0, 1), SnakeStraightBody = new Vector2(4, 1), SnakeTurnedBody = new Vector2(5, 0);

    private const int Snake = 0, Item = 1, Boundry = 2;
    [Export]
    public int ResolutionX, ResolutionY;
    [Export]
    public FaceTowards SnakeFaceTowards;
    private bool AddItem;
    [Export]
    public Vector2 SnakeStartPos;
    private Vector2 SnakeDirection;
#region Input consts
    private const string InputMoveUP = "Player_Move_Up", InputMoveDown = "Player_Move_Down", InputMoveRight = "Player_Move_Right", InputMoveLeft = "Player_Move_Left";
#endregion

    private Vector2 ItemPos;
    private List<Vector2> SnakeBody = new List<Vector2>();

    public override void _Ready()
    {
        Map = GetParent().GetNode<TileMap>("TileMap");
        SetUp();
    }

    public override void _Process(float delta)
    {
        CheckForGameOver();
    }

    private void SetUp()
    {
        Reset();

        ItemPos = PlaceItem();
    }

    private Vector2 PlaceItem()
    {
        int x, y;
        Random rand = new Random();

        x = rand.Next(0, ResolutionX);
        y = rand.Next(0, ResolutionY);
        return new Vector2(x, y);
    }

    private void DrawItem()
    {
        Map.SetCell((int)ItemPos.x, (int)ItemPos.y, Item);
    }

    private void DrawSnake()
    {
        //foreach (Vector2 Block in SnakeBody)
        //    Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, false, new Vector2(8, 0));
        Vector2 Block, PreviousBlock, NextBlock;
        for (int i = 0; i < SnakeBody.Count; i++)
        {
            Block = SnakeBody[i];
            //Head of Snake
            if (i == 0)
            {
                if (Relation2(SnakeBody[i], SnakeBody[i + 1]) == Vector2.Up) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, true, false, SnakeHead);
                if (Relation2(SnakeBody[i], SnakeBody[i + 1]) == Vector2.Down) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, false, SnakeHead);
                if (Relation2(SnakeBody[i], SnakeBody[i + 1]) == Vector2.Right) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, true, SnakeHead);
                if (Relation2(SnakeBody[i], SnakeBody[i + 1]) == Vector2.Left) Map.SetCell((int)Block.x, (int)Block.y, Snake, true, false, true, SnakeHead);
            }
            //Tail of Snake
            else if (i == SnakeBody.Count - 1)
            {
                if (Relation2(SnakeBody[i], SnakeBody[i - 1]) == Vector2.Up) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, true, false, SnakeTail);
                if (Relation2(SnakeBody[i], SnakeBody[i - 1]) == Vector2.Down) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, false, SnakeTail);
                if (Relation2(SnakeBody[i], SnakeBody[i - 1]) == Vector2.Right) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, true, SnakeTail);
                if (Relation2(SnakeBody[i], SnakeBody[i - 1]) == Vector2.Left) Map.SetCell((int)Block.x, (int)Block.y, Snake, true, false, true, SnakeTail);
            }
            else
            {
                PreviousBlock = SnakeBody[i + 1] - Block;
                NextBlock = SnakeBody[i - 1] - Block;

                if (PreviousBlock.x == NextBlock.x) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, false, SnakeStraightBody);
                else if(PreviousBlock.y == NextBlock.y) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, true, SnakeStraightBody);
                else
                {
                    if (PreviousBlock.x == -1 && NextBlock.y == -1 || PreviousBlock.y == -1 && NextBlock.x == -1) Map.SetCell((int)Block.x, (int)Block.y, Snake, true,true,false, SnakeTurnedBody);
                    if (PreviousBlock.x == -1 && NextBlock.y == 1 || PreviousBlock.y == 1 && NextBlock.x == -1) Map.SetCell((int)Block.x, (int)Block.y, Snake, true,false,false, SnakeTurnedBody);
                    if (PreviousBlock.x == 1 && NextBlock.y == -1 || PreviousBlock.y == -1 && NextBlock.x == 1) Map.SetCell((int)Block.x, (int)Block.y, Snake, false,true,false, SnakeTurnedBody);
                    if (PreviousBlock.x == 1 && NextBlock.y == 1 || PreviousBlock.y == 1 && NextBlock.x == 1) Map.SetCell((int)Block.x, (int)Block.y, Snake, false, false, false, SnakeTurnedBody);
                }
            }
        }
    }

    private Vector2 Relation2(Vector2 block1, Vector2 block2)
    {
        return block2 - block1;
    }

    private void MoveSnake()
    {
        if (AddItem)
        {
            DeleteTiles(Snake);
            List<Vector2> BodyCopy = SnakeBody.GetRange(0, SnakeBody.Count);
            Vector2 NewHead = SnakeBody[0] + SnakeDirection;
            BodyCopy.Insert(0, NewHead);
            SnakeBody = BodyCopy;
            AddItem = false;
        }
        else
        {
            DeleteTiles(Snake);
            List<Vector2> BodyCopy = SnakeBody.GetRange(0, SnakeBody.Count - 1);
            Vector2 NewHead = SnakeBody[0] + SnakeDirection;
            BodyCopy.Insert(0, NewHead);
            SnakeBody = BodyCopy;
        }
       
    }

    private void DeleteTiles(int id)
    {
        Godot.Collections.Array Cells = Map.GetUsedCellsById(id);
        foreach (Vector2 cell in Cells)
            Map.SetCell((int)cell.x, (int)cell.y, -1);
    }

    private Vector2[] SetTiles(int id)
    {
        Vector2[] array;
        Godot.Collections.Array Cells = Map.GetUsedCellsById(id);
        array = new Vector2[Cells.Count];
        for (int i = 0; i < Cells.Count; i++)
            array[i] = (Vector2) Cells[i];
        return array;
    }

    private Vector2 GetDirection(FaceTowards direction)
    {
        Vector2 Direction = Vector2.Zero;
        switch (direction)
        {
            case FaceTowards.Up:
                Direction = Vector2.Up;
                break;
            case FaceTowards.Down:
                Direction = Vector2.Down;
                break;
            case FaceTowards.Right:
                Direction = Vector2.Right;
                break;
            case FaceTowards.Left:
                Direction = Vector2.Left;
                break;
        }
        return Direction;
    }

    private void ItemInteraction()
    {
        if (ItemPos == SnakeBody[0])
        {
            ItemPos = PlaceItem();
            AddItem = true;
        }
    }

    public void _on_Timer_timeout()
    {
        MoveSnake();
        DrawItem();
        DrawSnake();
        ItemInteraction();
    }

    private void CheckForGameOver()
    {
        Vector2 head = SnakeBody[0];
        
        //If Snake Bites the boundry
        if (head.x < 0 || head.y < 0 || head.x > ResolutionX - 1 || head.y > ResolutionY - 1)
            Reset();
        //If Snake Bites itself
        foreach (Vector2 block in SnakeBody.GetRange(1, SnakeBody.Count - 1))
            if (block == head)
                Reset();
    }

    private void Reset()
    {
        SnakeBody.Clear();
        SnakeBody.Add(SnakeStartPos);
        SnakeBody.Add(SnakeBody[0] - SnakeDirection);
        SnakeBody.Add(SnakeBody[1] - SnakeDirection);
        
        SnakeDirection = GetDirection(SnakeFaceTowards);
    }

    public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed(InputMoveUP) && SnakeDirection != Vector2.Down) SnakeDirection = Vector2.Up;
		if (inputEvent.IsActionPressed(InputMoveDown) && SnakeDirection != Vector2.Up) SnakeDirection = Vector2.Down;
		if (inputEvent.IsActionPressed(InputMoveRight) && SnakeDirection != Vector2.Left) SnakeDirection = Vector2.Right;
		if (inputEvent.IsActionPressed(InputMoveLeft) && SnakeDirection != Vector2.Right) SnakeDirection = Vector2.Left;
	}
}
