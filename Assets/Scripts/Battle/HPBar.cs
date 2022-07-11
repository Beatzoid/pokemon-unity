using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    /// <summary>
    /// Set the HP of the HP bar
    /// </summary>
    /// <param name="hpNormalized">The HP (normalized) </param>
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    /// <summary>
    /// Smoothly set the HP of the HP bar
    /// </summary>
    /// <param name="newHp">The HP </param>
    public IEnumerator SetHPSmoothly(float newHp)
    {
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);
    }
}