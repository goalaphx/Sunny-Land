using UnityEngine;

public class InvincibilityPowerup : MonoBehaviour
{
    public float invincibilityDuration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplyInvincibility(invincibilityDuration);
            }
            Destroy(gameObject);
        }
    }
}
