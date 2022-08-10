using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectsPrefab;

    public void Awake()
    {
        EssentialObjects[] existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
