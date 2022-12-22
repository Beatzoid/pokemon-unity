using UnityEngine;

/// <summary>
/// The EssentialObjectsSpawner manages the initiating of the 
/// EssentialObjects gameobject
/// </summary>
public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectsPrefab;

    public void Awake()
    {
        EssentialObjects[] existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            Vector3 spawnPos = new Vector3(0, 0, 0);

            Grid grid = FindObjectOfType<Grid>();
            if (grid)
                spawnPos = grid.transform.position;

            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}
