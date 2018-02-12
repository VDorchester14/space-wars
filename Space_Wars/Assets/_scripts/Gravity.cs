using UnityEngine;
using System.Collections.Generic;

public class Gravity : MonoBehaviour
{
    public float range = 1000f;

    Rigidbody2D ownRb;

    void Start()
    {
        ownRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
        List<Rigidbody2D> rbs = new List<Rigidbody2D>();

        foreach (Collider2D c in cols)
        {
            Rigidbody2D rb = c.attachedRigidbody;
            //Debug.Log(c);
            if (rb != null && rb != ownRb && !rbs.Contains(rb))
            {
                rbs.Add(rb);
                Vector3 offset = transform.position - c.transform.position;
                rb.AddForce(offset / offset.sqrMagnitude * ownRb.mass);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
