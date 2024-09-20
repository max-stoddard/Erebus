using System.Collections.Generic;
using UnityEngine;

// Script that uses MoveTowards and pathfinding if in range of an obstacle to make any enemy follow the player

public class EnemyFollow : MonoBehaviour
{
    [Header("Pathfinding")]

    private float CurrentFollowSpeed;

    [SerializeField] private float DefaultFollowSpeed;

    public GameObject Target;

    private List<Vector2> Path;

    //Path is a list of world positions
    //Path[0] is the next position to go to
    //If Path is empty, enemy has reached position
    //If path is null, no possible path can be found


    private Pathfinding pf;

    private bool IsPathfinding; // When pathfinding is off the enemy takes a direct route to destination

    private Vector2 CurrentGridPosition;


    [Header("Rotation")]

    [SerializeField] private float RotationSpeed;

    private float OldAngle;



    [Header("Seperation")]

    [SerializeField] private float DefaultSeperateSpeed;

    [SerializeField] private float SeperateRadius;


    [Header("References")]

    private Rigidbody2D rb;

    private Rigidbody2D rbTarget;


    [Header("Debug")]

    [SerializeField] private Material DebugMaterial;

    private List<GameObject> Lines;

    void Start()
    {
        CurrentFollowSpeed = DefaultFollowSpeed;
        pf = Pathfinding.g;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rbTarget = Target.GetComponent<Rigidbody2D>();
        Path = FindPathToTarget();
       
        
        IsPathfinding = true;
        Lines = new List<GameObject>();
    }

    void Update()
    {
        // Toggle isPathfinding

        // If endnode is where player is standing can force path

        //Pathfind if player or enemy is on pathfinding node

        CheckTargetMovedGridPosition();

        if (!IsPathfinding && (pf.ShouldPathfindOnPosition(rb.position) || pf.ShouldPathfindOnPosition(rbTarget.position)))
        {
            IsPathfinding = true;
            Path = FindPathToTarget();
            if (pf.Visualize) VisulizePath();

        }

        // Dont pathfind if neither player or enemy is on pathfinding node
        else if (IsPathfinding && !pf.ShouldPathfindOnPosition(rb.position) && !pf.ShouldPathfindOnPosition(rbTarget.position))
        {
            IsPathfinding = false;
            Path = null;
            if (pf.Visualize) VisulizePath();

            return;
        }

        if (Path == null)
        {
            return;
        }
        if (Path.Count == 0) // Set null if path is empty
        {
            Path = null;
            return;
        }

        if ((rb.position - Path[0]).magnitude < .5f) // If enemy has reached a node
        {
            Path.RemoveAt(0);

            if (pf.Visualize) VisulizePath();

        }


    }

    private void FixedUpdate()
    {
        Vector2 TargetPosition;
        if (IsPathfinding && Path != null && Path.Count > 0)
        {
            TargetPosition = Path[0];
        }
        else
        {
            TargetPosition = Target.GetComponent<Rigidbody2D>().position;
        }

        Vector2 MoveAwayDirection = GetDirectionToMoveAwayFromEnemies();

        Vector2 NextPosition = Vector2.MoveTowards(rb.position, TargetPosition, Time.fixedDeltaTime * CurrentFollowSpeed) + DefaultSeperateSpeed * Time.fixedDeltaTime * MoveAwayDirection;

        rb.MovePosition(NextPosition);



        float NewAngle = FindNewAngle(NextPosition);

        OldAngle = NewAngle;

        rb.SetRotation(NewAngle);
    }

    private void CheckTargetMovedGridPosition()
    {
        if (CurrentGridPosition != Pathfinding.g.Grid.GetXY(Target.transform.position))
        {
            CurrentGridPosition = Pathfinding.g.Grid.GetXY(Target.transform.position);
            RequestNewPath();
        }
    }

    private float FindNewAngle(Vector2 NextPosition)
    {
        // - angles turn clockwise
        // + angles turn anticlockwise
        /*
        OLD    TARGET   O - T   DIR
         20      10      10     -1
         10      20     -10      1
        -10     -20      10     -1
        -20     -10     -10      1
         10     -10      20     -1
        -10      10     -20      1
        -170     170    -340    -1
         170    -170     340     1
         170    160      10      -1
        */

        Vector2 EnemyToPlayer = NextPosition - rb.position;
        float TargetAngle = Mathf.Atan2(EnemyToPlayer.y, EnemyToPlayer.x) * Mathf.Rad2Deg - 90f; // Between -180 and 180
        if (TargetAngle < -180) TargetAngle += 360f;


        int dir;
        float dif = OldAngle - TargetAngle;

        if (dif == 0 || Mathf.Abs(dif) <= 1 * RotationSpeed * Time.fixedDeltaTime)
        {
            dir = 0;
        }
        else if (Mathf.Abs(dif) < 180)
        {
            if (dif > 0) dir = -1;
            else dir = 1;
        }
        else
        {
            if (dif > 0) dir = 1;
            else dir = -1;
        }

        float NewAngle = OldAngle + dir * RotationSpeed * Time.fixedDeltaTime;

        if (NewAngle < -180) NewAngle += 360;
        if (NewAngle > 180) NewAngle -= 360;

        return NewAngle;
    }

    private Vector2 GetDirectionToMoveAwayFromEnemies() // OBJECTIVE 18
    {
        Vector2 sum = Vector2.zero;
        var hits = Physics2D.OverlapCircleAll(rb.position, SeperateRadius);
        int EnemyCount = 0;

        foreach (var hit in hits)
        {
            if (hit.GetComponent<EnemyFollow>() != null && hit.transform != transform) // check hit is enemy not gameobject
            {

                Vector2 DirectionEnemyToThis = (transform.position - hit.transform.position).normalized; // This gets the normalised direction to move away from current enemy
                float DistanceToEnemy = Mathf.Abs((transform.position - hit.transform.position).magnitude); // This is the distance to current enemy
                Vector2 SuggestedMovement = DirectionEnemyToThis / DistanceToEnemy; // Suggested movement is more if closer to current enemy
                sum += SuggestedMovement; // Added to a vector2 sum
                EnemyCount++;
            }
        }

        if (EnemyCount > 0)
        {
            sum = (sum / EnemyCount).normalized; //Finds direction of the sum of all these enemies
        }

        return sum;
    }

    private void RequestNewPath()
    {
        if (IsPathfinding)
        {
            List<Vector2> NewPath = FindPathToTarget();
            if (NewPath != null)
            {
                Path = NewPath;
            }
        }
        if (pf.Visualize) VisulizePath();
    }

    private List<Vector2> FindPathToTarget()
    {
        List<Vector2> path = pf.FindPath(rb.position, Target.transform.position);
        if (path != null)
        {
            path.RemoveAt(0); // Remove path to center of current node
            if (path.Count > 0)
            {
                path.RemoveAt(path.Count - 1); // Remove to center of final node
            }
            return path;
        }
        return null;
    }

    public void SetSpeedToPercent(float percent)
    {
        if (percent >= 0)
        {
            DefaultFollowSpeed *= percent;
        }
    }

    public void SetTarget(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError($"Target null");
        }
        Target = target;
    }

    private void VisulizePath()
    {
        if (Lines == null)
        {
            Debug.LogError("Lines is null");
            return;
        }
        foreach (var line in Lines)
        {
            Object.Destroy(line);
        }

        if (Path != null && Path.Count > 0)
        {
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Vector2 startPos = Path[i];
                Vector2 endPos = Path[i + 1];

                CreateLine(startPos, endPos);
            }

            CreateLine(Path[Path.Count - 1], Target.transform.position);
        }
        else
        {
            Vector2 startPos = rb.transform.position;
            Vector2 endPos = Target.transform.position;



            CreateLine(startPos, endPos);
        }
    }

    private void CreateLine(Vector2 StartPos, Vector2 EndPos)
    {
        GameObject go = new GameObject
        {
            name = $"Debug Line",
            tag = "Line"
        };
        go.transform.SetParent(GameObject.FindGameObjectWithTag("Debug").transform);
        Lines.Add(go);
        LineRenderer l = go.AddComponent<LineRenderer>();
        l.SetPosition(0, StartPos);
        l.SetPosition(1, EndPos);
        l.startColor = l.endColor = Color.blue;
        l.sortingLayerName = "Top";
        l.material = DebugMaterial;
        l.startWidth = l.endWidth = .075f;
    }

    public void Die()
    {
        foreach (var line in Lines)
        {
            Object.Destroy(line);
        }
        Destroy(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (transform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SeperateRadius);
    }

}
