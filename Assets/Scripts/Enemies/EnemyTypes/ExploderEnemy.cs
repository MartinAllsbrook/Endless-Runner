using System.Collections;
using UnityEngine;

class ExploderEnemy : Enemy
{
    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        health.DecreaseHealth(health.CurrentHealth); // Destroy self after short delay
    }
}