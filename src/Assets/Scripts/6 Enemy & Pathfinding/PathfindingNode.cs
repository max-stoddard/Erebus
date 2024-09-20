using System.Collections.Generic;

public class PathfindingNode
{
    public int X { get; } // X position of node
    public int Y { get; } // Y position of node

    public int G_Cost { get; set; } // Cost from start node
    public int H_Cost { get; set; } // Direct route to end node
    public int F_Cost { get; private set; } // = G + H (lower is better)

    // Nodes are either non walkable, pathfinding or 
    public bool ShouldWalkOnNode { get; set; } // Non walkable nodes are nodes which centre is within an obstacle
    public bool ShouldPathfindNode { get; set; } // Pathfinding nodes are nodes with a proximity to non walkable nodes

    public PathfindingNode NodeCameFrom { get; set; }
    public List<PathfindingNode> Neighbours { get; set; }


    public PathfindingNode(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.ShouldWalkOnNode = true;
        this.ShouldPathfindNode = false;
    }

    public void CalculateFCost()
    {
        F_Cost = G_Cost + H_Cost;
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }

    public bool IsBlockedNode() // RED: Node which is blocked by obstacle
    {
        if (!ShouldWalkOnNode && !ShouldPathfindNode) return true;
        return false;
    }

    public bool IsNonWalkableNode() // YELLOW: Node which is in close proximity to obstacle and shouldn't be pathfinded onto,
                                    // but if on this area pathfinding should attempt to get entity out of this node
    {
        if (!ShouldWalkOnNode && ShouldPathfindNode) return true;
        return false;
    }

    public bool IsPathfindingNode() // GREEN: Node which is close to obstacle but not too close so pathfinding should occur
    {
        if (ShouldWalkOnNode && ShouldPathfindNode) return true;
        return false;
    }

    public bool IsDefaultNode() // WHTE: Node which doesn't have any objects near it
    {
        if (ShouldWalkOnNode && !ShouldPathfindNode) return true;
        return false;
    }
}
