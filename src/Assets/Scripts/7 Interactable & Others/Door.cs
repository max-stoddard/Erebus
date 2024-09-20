using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private int Price;
    [SerializeField] private int ID;

    protected override void Start()
    {
        base.Start();
        CreatePopup();
        CurrentCost = Price;

        Door[] doors = FindObjectsOfType<Door>();
        foreach (Door door in doors)
        {
            if (door.ID == ID && door.gameObject != gameObject)
            {
                Debug.LogError($"Door ID duplicaiton occured with {door.gameObject} & {gameObject}");
            }
        }

        if (ID < 0 || ID >= doors.Length)
        {
            Debug.LogError($"Door ID out of range: {ID}");
        }
    }

    protected override void Interact()
    {
        gameObject.tag = "Untagged"; // So reset walkable nodes works
        SpendMoneyCurrentCost(); // OBJECTIVE 22.5
        RoundManager.g.DoorOpen(ID); // OBJECTIVE 28.3
        Pathfinding.g.ResetWalkableNodes();
        DestoryInteractable();
    }

    protected override void CreatePopup()
    {
        base.CreatePopup();
        bps.ChangeMainMessage("OPEN DOOR");
        bps.ChangePrice(Price);
        bps.ChangeImage(null);
        bps.ChangeSideMessage("");
    }


}
