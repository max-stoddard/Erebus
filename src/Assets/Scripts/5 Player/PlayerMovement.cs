using UnityEngine;

// Made by Max Stoddard - Started 15/01/22
public class PlayerMovement : MonoBehaviour // OBJECTIVE 13
{
    [Header("Input")]

    private InputActions PlayerInputActions; // Instance of PlayerInputActions to handle player input

    [Header("Movement")]
    [SerializeField] private float DefaultMoveSpeed;
    private float MoveSpeed;


    private Rigidbody2D RB; // Reference to collider attribute of the player
    private Vector2 MovementInputVector;

    

    private bool FastMove;

    [SerializeField] private bool DebugSpeed;

    private void Start() // Executed before gameplay starts
    {
        RB = GetComponent<Rigidbody2D>(); // Gets the RB2D

        PlayerInputActions = GameAssets.g.PlayerInputActions; // Creates instance of our PlayerInputMap for our script
        PlayerInputActions.Player.Movement.Enable(); // Enables player input map
        MoveSpeed = DebugSpeed ? 1000f : DefaultMoveSpeed;
        FastMove = false;
    }

    private void Update()
    {
        ProcessMovementInputs();
        CheckForPowerup();
    }

    private void CheckForPowerup()
    {
        if (!FastMove && GameAssets.g.PowerupManager.IsPowerupActive(0)) // OBJECTIVE 13.1.4
        {
            FastMove = true;
            MultiplyMoveSpeed(1.2f);
        }
    }

    private void FixedUpdate() // Fixed update: for physics & called set number of times per second regardless of framerate
    {
        RB.velocity = MovementInputVector * Time.fixedDeltaTime; // fixedDeltaTime is time between last frame in FixedUpdate
    }

    private void ProcessMovementInputs() // OBJECTIVE 13.1
    {
        // Gets vector of user movement input
        MovementInputVector = PlayerInputActions.Player.Movement.ReadValue<Vector2>();
        // velocity of movement is constant independent of direction
        MovementInputVector.Normalize(); // OBJECTIVE 13.1.2
        // Varies movement speed based on float value
        MovementInputVector *= MoveSpeed;
    }

    public void MultiplyMoveSpeed(float Multiply)
    {
        if (Multiply <= 0)
        {
            Debug.LogWarning($"Invalid multiply speed: {Multiply}");
            return;
        }
        MoveSpeed = Multiply * DefaultMoveSpeed;
    }

}
