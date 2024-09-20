using System;
using System.Collections.Generic;
using UnityEngine;

// OBJECTIVE 16.1
public class Graph<GridType> // Allows creation of any type / class
{
    public int Width { get; } // Width

    public int Height { get; } // Height

    public float NodeSize { get; } // Node Size

    private Vector2 OriginPosition { get; } // Origin Position

    public GridType[,] GraphDataArray { get; }

    public Graph(int W, int H, float NodeSize, Vector2 OriginPos, Func<Graph<GridType>, int, int, GridType> CreateDefaultGridObject)
    {
        this.Width = W;
        this.Height = H;
        this.NodeSize = NodeSize;
        this.OriginPosition = OriginPos;

        GraphDataArray = new GridType[W, H];

        for (int x = 0; x < GraphDataArray.GetLength(0); x++) // 0 to length - 1
        {
            for (int y = 0; y < GraphDataArray.GetLength(1); y++) // 0 to length - 1
            {
                GraphDataArray[x, y] = CreateDefaultGridObject(this, x, y);
            }
        }

    }

    public Vector2 GetSW_WorldPosition(int x, int y) // Gets world position from node coords
    {
        return new Vector2(x, y) * NodeSize + OriginPosition;
    }

    public Vector2 GetCenterWorldPosition(int x, int y) // Gets world position of center of square from node coords
    {
        return GetSW_WorldPosition(x, y) + new Vector2(NodeSize, NodeSize) * .5f;
    }

    public Vector2 GetNE_WorldPosition(int x, int y)
    {
        return GetSW_WorldPosition(x, y) + new Vector2(NodeSize, NodeSize);
    } // Gets world position of top right corner of node

    public void GetXY(Vector2 WorldPosition, out int x, out int y) // Out allows the return of multiple varibles
    {
        x = Mathf.FloorToInt((WorldPosition - OriginPosition).x / NodeSize); // Takes lower int of float
        y = Mathf.FloorToInt((WorldPosition - OriginPosition).y / NodeSize); // Takes lower int of float
    }

    public Vector2 GetXY(Vector2 WorldPosition)
    {
        GetXY(WorldPosition, out int x, out int y);
        return new Vector2(x, y);
    }

    private void GetLowerXY(Vector2 WorldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(((WorldPosition - OriginPosition).x / NodeSize) - 0.001f); // Takes lower int of float
        y = Mathf.FloorToInt(((WorldPosition - OriginPosition).y / NodeSize) - 0.001f); // Takes lower int of float
    }

    public void SetNodeData(int x, int y, GridType value)
    {
        if (CoordsIsValid(x, y)) //Is input valid (in the array)
        {
            GraphDataArray[x, y] = value;
        }
    } // Sets the node data from the x and y for a select node

    public void SetNodeData(Vector2 WorldPos, GridType value)
    {
        GetXY(WorldPos, out int x, out int y);
        SetNodeData(x, y, value);
    } // Sets the node data from a world position

    public GridType GetNodeData(int x, int y)
    {
        if (CoordsIsValid(x, y)) //Is input valid (in the array)
        {
            return GraphDataArray[x, y];
        }
        else
        {
            return default;
        }
    } // Returns the data within a node from x and y

    public GridType GetNodeData(Vector2 WorldPos)
    {
        GetXY(WorldPos, out int x, out int y);
        return GetNodeData(x, y);
    } // Returns the data within a node from a world position

    public bool CoordsIsValid(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            return true;
        }
        return false;
    }// Checks whether coordinates are within the bounds of the graph

    public List<GridType> GetAllNodesInBound(Bounds bounds, float radius)
    {
        List<GridType> Nodes = new List<GridType>();


        GetXY(bounds.min - new Vector3(radius, radius, 0), out int x1, out int y1);
        GetLowerXY(bounds.max + new Vector3(radius, radius, 0), out int x2, out int y2);

        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                if (CoordsIsValid((int)x, (int)y))
                {
                    Nodes.Add(GetNodeData(x, y));
                }
            }
        }
        return Nodes;

    }// returns a list of all nodes in bound + a radius
}
