using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour // OBJECTIVE 16
{
    [Header("Grid")]

    [SerializeField] private int Width;
    [SerializeField] private int Height;
    [SerializeField] private float NodeSize;
    [SerializeField] private Vector2 StartPosition;


    [Header("Pathfinding")]


    [SerializeField] private float ObstacleRadius; // The radius around obstacles of which cannot be walked to
    [SerializeField] private float ShouldntWalkRadius;
    [SerializeField] private float PathfindingRadius; // The radius around obstacles of which pathfinding will be used

    public Graph<PathfindingNode> Grid { get; private set; }
    private List<PathfindingNode> NodesToSearch; //  Nodes queued up for searching 
    private HashSet<PathfindingNode> NodesSearched; // Nodes already been searched (hashset as only data needed is whether node exists)

    private const int StraightCost = 10;
    private const int DiagonalCost = 14;


    [Header("Debug")]

    [SerializeField] private bool VisualizeText;
    public bool Visualize;

    private TextMesh[,] GraphDebugTextArray;

    public static Pathfinding g { get; private set; }


    void Awake()
    {
        // One pathfinding grid is needed for all scene; called a singleton
        g = this;

    // Creating grid
        Grid = new Graph<PathfindingNode>(Width, Height, NodeSize, StartPosition, (Graph<PathfindingNode> g, int x, int y) => new PathfindingNode(x, y));
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                PathfindingNode node = Grid.GetNodeData(x, y);
                node.Neighbours = GetAllNeighbours(node);
            }
        }
        ResetWalkableNodes();

        // Debugging
        if (Visualize)
        {
            VisulizePathfinding();
        }

        if (VisualizeText)
        {
            Visualize_Text();
        }

        Debug.DrawLine(Grid.GetSW_WorldPosition(0, 0), Grid.GetSW_WorldPosition(0, Height), Color.cyan, 100f, false);
        Debug.DrawLine(Grid.GetSW_WorldPosition(0, 0), Grid.GetSW_WorldPosition(Width, 0), Color.cyan, 100f, false);
        Debug.DrawLine(Grid.GetSW_WorldPosition(Width, 0), Grid.GetSW_WorldPosition(Width, Height), Color.cyan, 100f, false);
        Debug.DrawLine(Grid.GetSW_WorldPosition(0, Height), Grid.GetSW_WorldPosition(Width, Height), Color.cyan, 100f, false);

    }

    #region Find Path Methods

    public List<Vector2> FindPath(Vector2 StartWorldPosition, Vector2 EndWorldPoisition)
    {
        Grid.GetXY(StartWorldPosition, out int X1, out int Y1);
        Grid.GetXY(EndWorldPoisition, out int X2, out int Y2);

        List<PathfindingNode> path = FindPath(X1, Y1, X2, Y2);

        if (path == null)
        {
            return null;
        }

        List<Vector2> VectorPath = new List<Vector2>();
        foreach (PathfindingNode node in path)
        {
            VectorPath.Add(Grid.GetCenterWorldPosition(node.X, node.Y));
        }

        return VectorPath;
    }

    public List<PathfindingNode> FindPath(int x1, int y1, int x2, int y2) // MAIN FUNCTION: finds a path from (x1, y1) => (x2, y2) 
    {
        if (!Grid.CoordsIsValid(x1, y1) || !Grid.CoordsIsValid(x2, y2))
        {
            return null;
        }

        // Initialising Start and End nodes
        PathfindingNode StartNode = Grid.GetNodeData(x1, y1);
        PathfindingNode EndNode = Grid.GetNodeData(x2, y2);

        // Initialising lists for nodes to be and been searched
        NodesToSearch = new List<PathfindingNode> { StartNode }; // StartNode in open nodes as open nodes is list we need to search
        NodesSearched = new HashSet<PathfindingNode>(); // Empty

        // Initialising every nodes gcost 
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                PathfindingNode Node = Grid.GetNodeData(x, y);
                Node.G_Cost = int.MaxValue; // Setting cost from start to max as default
                Node.CalculateFCost(); // F cost = max value
                Node.NodeCameFrom = null;
            }
        }

        // Initialising start node 
        StartNode.G_Cost = 0;
        StartNode.H_Cost = CalculateDirectDistanceCost(StartNode, EndNode);
        StartNode.CalculateFCost();

        // Cycle

        //  Do Until
        //      Current Node = End Node
        //  OR
        //      Open List is empty (no path)

        while (NodesToSearch.Count > 0) // While there are nodes to search
        {
            PathfindingNode CurrentNode = GetLowestFCostNode(NodesToSearch); // Current node is node with best fcost; at start must be start node

            if (CurrentNode == EndNode) // If endnode reached return path
            {
                return RetracePath(EndNode);
            }

            // Node searched and is not end node
            NodesToSearch.Remove(CurrentNode);
            NodesSearched.Add(CurrentNode);


            foreach (PathfindingNode NeighbourNode in CurrentNode.Neighbours)
            {
                if (NodesSearched.Contains(NeighbourNode)) continue;
                if (!(NeighbourNode.ShouldWalkOnNode || NeighbourNode == EndNode)) // If isn't walkable but isn't EndNode, remove node from search
                {
                    NodesSearched.Add(NeighbourNode);
                    continue;
                }

                // start -> current node + current node -> neighbournode 
                int NeighbourGCost = CurrentNode.G_Cost + CalculateDirectDistanceCost(CurrentNode, NeighbourNode);
                if (NeighbourGCost < NeighbourNode.G_Cost)
                {
                    NeighbourNode.NodeCameFrom = CurrentNode;
                    NeighbourNode.G_Cost = NeighbourGCost;
                    NeighbourNode.H_Cost = CalculateDirectDistanceCost(NeighbourNode, EndNode);
                    NeighbourNode.CalculateFCost();

                    if (!NodesToSearch.Contains(NeighbourNode))
                    {
                        NodesToSearch.Add(NeighbourNode);
                    }
                }

            }
        }
        return null;
    }

    private List<PathfindingNode> RetracePath(PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode> { endNode }; // Creates path starting with end node

        PathfindingNode currentNode = endNode;

        while (currentNode.NodeCameFrom != null)
        {
            path.Add(currentNode.NodeCameFrom);
            currentNode = currentNode.NodeCameFrom;
        }
        path.Reverse();

        return path;
    }

    #endregion

    #region Node Methods

    private int CalculateDirectDistanceCost(PathfindingNode a, PathfindingNode b)
    {
        int xDistance = Mathf.Abs(a.X - b.X); // Horizontal distance between nodes
        int yDistance = Mathf.Abs(a.Y - b.Y); // Verical distance 
        int xyDifference = Mathf.Abs(xDistance - yDistance);
        return DiagonalCost * Mathf.Min(xDistance, yDistance) + StraightCost * xyDifference;
    }

    private PathfindingNode GetLowestFCostNode(List<PathfindingNode> pathfindingNodes)
    {
        PathfindingNode lowestFCostNode = pathfindingNodes[0];
        for (int i = 1; i < pathfindingNodes.Count; i++)
        {
            if (pathfindingNodes[i].F_Cost < lowestFCostNode.F_Cost)
            {
                lowestFCostNode = pathfindingNodes[i];
            }
        }
        return lowestFCostNode;
    }

    private List<PathfindingNode> GetAllNeighbours(PathfindingNode Node)
    {
        /*
        Need N, NE, E, SE, S, SW, W, NW
        
        
        0Y              XY

            NW  NN  NE

            WW  ##  EE
         
            SW  SS  SE

        00              X0


        */
        List<PathfindingNode> NeighbourList = new List<PathfindingNode>();

        bool N = Node.Y + 1 < Grid.Height;
        bool E = Node.X + 1 < Grid.Width;
        bool S = Node.Y - 1 >= 0;
        bool W = Node.X - 1 >= 0;

        if (N)
        {
            // N
            NeighbourList.Add(Grid.GetNodeData(Node.X, Node.Y + 1));

            if (E)
            {
                //NE
                NeighbourList.Add(Grid.GetNodeData(Node.X + 1, Node.Y + 1));
            }
            if (W)
            {
                //NW
                NeighbourList.Add(Grid.GetNodeData(Node.X - 1, Node.Y + 1));
            }

        }
        if (S)
        {
            // S
            NeighbourList.Add(Grid.GetNodeData(Node.X, Node.Y - 1));

            if (E)
            {
                // SE
                NeighbourList.Add(Grid.GetNodeData(Node.X + 1, Node.Y - 1));
            }
            if (W)
            {
                // SW
                NeighbourList.Add(Grid.GetNodeData(Node.X - 1, Node.Y - 1));
            }
        }
        if (E)
        {
            NeighbourList.Add(Grid.GetNodeData(Node.X + 1, Node.Y));
        }
        if (W)
        {
            NeighbourList.Add(Grid.GetNodeData(Node.X - 1, Node.Y));
        }

        return NeighbourList;
    }

    private void SetWalakbleNodes(float WalkableRadius, float WalkableAvoidRadius, float PathfindingRadius)
    {
        // 1) Gets all structure colliders
        GameObject[] AllStructures = GameObject.FindGameObjectsWithTag("Structure");
        List<Collider2D> StructureColliders = new List<Collider2D>();
        if (AllStructures != null)
        {
            foreach (GameObject Structure in AllStructures)
            {
                if (Structure != null && Structure.GetComponent<Transform>() != null && Structure.GetComponent<Collider2D>() != null)
                {
                    StructureColliders.Add(Structure.GetComponent<Collider2D>());
                }
            }
        }

        List<PathfindingNode> AllNodes = new List<PathfindingNode>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                AllNodes.Add(Grid.GetNodeData(x, y));
            }
        }
        foreach (PathfindingNode node in AllNodes)
        {
            node.ShouldWalkOnNode = true;
            node.ShouldPathfindNode = false;
        } // by default

        foreach (Collider2D collider2D in StructureColliders)
        {
            Bounds bounds = collider2D.bounds;

            List<PathfindingNode> ColliderNodes = Grid.GetAllNodesInBound(bounds, WalkableRadius);
            foreach (PathfindingNode node in ColliderNodes)
            {
                node.ShouldWalkOnNode = false;
                node.ShouldPathfindNode = false;
            }

            List<PathfindingNode> walkableNonPathfindingNodes = Grid.GetAllNodesInBound(bounds, WalkableAvoidRadius);
            foreach (PathfindingNode node in walkableNonPathfindingNodes) if (node.ShouldWalkOnNode)
                {
                    node.ShouldWalkOnNode = false;
                    node.ShouldPathfindNode = true;
                }

            List<PathfindingNode> pathfindingNodes = Grid.GetAllNodesInBound(bounds, PathfindingRadius);
            foreach (PathfindingNode node in pathfindingNodes) if (node.ShouldWalkOnNode)
                {
                    node.ShouldPathfindNode = true;
                    node.ShouldWalkOnNode = true;
                }



        }
    }// Detects whether nodes are walkable and should pathfind on them

    public void ResetWalkableNodes()
    {
        SetWalakbleNodes(ObstacleRadius, ShouldntWalkRadius, PathfindingRadius);
        if (Visualize)
        {
            VisulizePathfinding();
        }
    }

    public bool ShouldPathfindOnPosition(Vector2 WorldPosition)
    {
        Grid.GetXY(WorldPosition, out int x, out int y);
        if (Grid.CoordsIsValid(x, y) && Grid.GetNodeData(x, y).ShouldPathfindNode) return true;
        return false;
    }

    #endregion

    #region Debug

    private void VisulizePathfinding()
    {
        float time = 1000f;

        for (int x = 1; x < Width; x++)
        {
            Debug.DrawLine(Grid.GetSW_WorldPosition(x, 0), Grid.GetSW_WorldPosition(x, Height), Color.white, time, false);
        }
        for (int y = 1; y < Height; y++)
        {
            Debug.DrawLine(Grid.GetSW_WorldPosition(0, y), Grid.GetSW_WorldPosition(Grid.Width, y), Color.white, time, false);
        }
        // Iterates through all x and y values
        for (int x = 0; x < Grid.GraphDataArray.GetLength(0); x++) // 0 to length - 1
        {
            for (int y = 0; y < Grid.GraphDataArray.GetLength(1); y++) // 0 to length - 1
            {
                if (Grid.GraphDataArray[x, y].IsBlockedNode())
                {
                    Debug.DrawLine(Grid.GetSW_WorldPosition(x, y), Grid.GetSW_WorldPosition(x + 1, y + 1), Color.red, time, false);
                }

                else if (Grid.GraphDataArray[x, y].IsNonWalkableNode())
                {
                    Debug.DrawLine(Grid.GetSW_WorldPosition(x, y), Grid.GetSW_WorldPosition(x + 1, y + 1), Color.yellow, time, false);
                }

                else if (Grid.GraphDataArray[x, y].IsPathfindingNode())
                {
                    Debug.DrawLine(Grid.GetSW_WorldPosition(x, y), Grid.GetSW_WorldPosition(x + 1, y + 1), Color.green, time, false);
                }
            }
        }
    }

    private void Visualize_Text()
    {
        int TextSize = Mathf.FloorToInt(Grid.NodeSize * 70);
        GraphDebugTextArray = new TextMesh[Width, Height];

        // Iterates through all x and y values
        for (int x = 0; x < Grid.GraphDataArray.GetLength(0); x++) // 0 to length - 1
        {
            for (int y = 0; y < Grid.GraphDataArray.GetLength(1); y++) // 0 to length - 1
            {

                GraphDebugTextArray[x, y] = CreateText(Grid.GraphDataArray[x, y].ToString(), Grid.GetCenterWorldPosition(x, y), TextSize);
                GraphDebugTextArray[x, y].gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Debug").transform);

                if (Grid.GraphDataArray[x, y].IsBlockedNode())
                {
                    GraphDebugTextArray[x, y].color = Color.red;
                }

                else if (Grid.GraphDataArray[x, y].IsNonWalkableNode())
                {
                    GraphDebugTextArray[x, y].color = Color.yellow;
                }

                else if (Grid.GraphDataArray[x, y].IsPathfindingNode())
                {
                    GraphDebugTextArray[x, y].color = Color.green;
                }

                else if (Grid.GraphDataArray[x, y].IsDefaultNode())
                {
                    GraphDebugTextArray[x, y].color = Color.white;
                }




            }
        }
    }

    private TextMesh CreateText(string text, Vector2 position, int size)
    {
        // Game Object
        GameObject go = new GameObject(text, typeof(TextMesh));
        Transform transform = go.transform;
        transform.localPosition = position;

        //Textmesh
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.text = text;
        textMesh.fontSize = size;
        textMesh.characterSize = .05f;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 1000;
        textMesh.GetComponent<MeshRenderer>().sortingLayerName = "Top";

        return textMesh;
    }

    #endregion
}