using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDown : MonoBehaviour
{
    public float multiplier = 1.5f;
    public float duration = 4.0f;

    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("RubyController"))
        {
            StartCoroutine(Trap(other));
        }
    }

    IEnumerator Trap(Collider2D player)
    {
        Instantiate(pickupEffect, transform.position, transform.rotation);

        RubyController stats = player.GetComponent<RubyController>();
        stats.speed /= multiplier;

        yield return new WaitForSeconds(duration);

        stats.speed *= multiplier;
    }
}
