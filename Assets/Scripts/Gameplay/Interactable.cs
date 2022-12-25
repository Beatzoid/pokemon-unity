using UnityEngine;
using System.Collections;

public interface Interactable
{
    IEnumerator Interact(Transform initiator);
}
