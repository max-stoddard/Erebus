using UnityEngine;

public class UniversalScene : MonoBehaviour
{
    [SerializeField] private GameObject[] KeepInEveryScene;

    private void Awake()
    {
        foreach (var go in KeepInEveryScene)
        {
            Instantiate(go);
        }
    }


}
