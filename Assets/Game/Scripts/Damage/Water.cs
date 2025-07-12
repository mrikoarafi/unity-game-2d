using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float deathYPosition = -2f; // Posisi Y saat player mati

    // Dipanggil saat objek lain masuk ke trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah objek yang masuk adalah Player
        if (collision.tag == "Player")
        {
            Health playerHealth = collision.GetComponent<Health>();

            // Set damage sama dengan max health agar langsung mati
            float damage = playerHealth.GetMaxHealth();

            // Freeze posisi player saat akan mati
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
                playerRigidbody.isKinematic = true;
            }

            // Ubah posisi Y player saat mati
            Vector3 currentPos = collision.transform.position;
            collision.transform.position = new Vector3(currentPos.x, deathYPosition, currentPos.z);

            // Berikan damage ke player (langsung mati)
            playerHealth.TakeDamage(damage);
        }
    }
}
