using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 5f;  // Jarak patrol
    [SerializeField] private float moveSpeed = 2f;       // Kecepatan patrol

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;     // Jarak serangan ke depan
    [SerializeField] private float attackCooldown = 2f;  // Cooldown antar serangan
    [SerializeField] private int damage = 1;             // Damage yang diberikan

    private Animator anim;
    private Vector3 startPosition;
    private bool movingRight = true;
    private float cooldownTimer = Mathf.Infinity;
    private bool isAttacking = false; // Status sedang menyerang
    private AudioSource attackAudioSource; // Audio Source untuk attack sound (posisi 1)

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        // Mengambil Audio Source kedua untuk attack sound
        AudioSource[] audioSources = GetComponents<AudioSource>();

        attackAudioSource = audioSources[1]; // Audio Source kedua untuk attack (index 1)
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Cek apakah bisa menyerang player
        if (CanAttackPlayer() && cooldownTimer > attackCooldown && !isAttacking)
        {
            Attack();
        }
        else if (!isAttacking && !CanAttackPlayer())
        {
            // Patrol hanya jika tidak sedang menyerang DAN player tidak dalam range
            Patrol();
        }
        // Jika player masih dalam range setelah attack, tunggu cooldown untuk attack lagi
    }

    // Patrol bolak-balik
    private void Patrol()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= startPosition.x + patrolDistance)
            {
                movingRight = false;
                FlipSprite();
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= startPosition.x)
            {
                movingRight = true;
                FlipSprite();
            }
        }
    }

    // Mengecek apakah player berada dalam jangkauan serangan enemy
    private bool CanAttackPlayer()
    {
        // Menentukan arah raycast berdasarkan arah pergerakan enemy
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        // Posisi awal raycast - dimulai dari depan enemy untuk menghindari self-detection
        Vector2 rayStart = (Vector2)transform.position + direction * 0.5f + Vector2.up * 0.3f;

        // Melakukan raycast untuk mencari semua objek dalam jangkauan
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, direction, attackRange);

        // Iterasi semua objek yang terkena raycast
        foreach (RaycastHit2D hit in hits)
        {
            // Skip jika raycast mengenai enemy sendiri
            if (hit.collider.gameObject == gameObject) continue;

            // Return true jika menemukan player
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false; // Player tidak ditemukan dalam jangkauan
    }

    // Menyerang player
    private void Attack()
    {
        isAttacking = true; // Set status attacking
        anim.SetTrigger("slash");
        cooldownTimer = 0;

        // Mainkan attack sound
        attackAudioSource.Play();

        DamagePlayer();

        // Reset status attacking setelah animasi selesai
        Invoke("ResetAttacking", 1f); // Sesuaikan dengan durasi animasi slash
    }

    // Reset status attacking
    private void ResetAttacking()
    {
        isAttacking = false;
        // Tidak langsung patrol, biarkan Update() cek apakah player masih dalam range
    }

    // Memberikan damage kepada player yang berada dalam jangkauan serangan
    private void DamagePlayer()
    {
        // Menentukan arah raycast berdasarkan arah pergerakan enemy
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        // Posisi awal raycast - sama dengan CanAttackPlayer untuk konsistensi
        Vector2 rayStart = (Vector2)transform.position + direction * 0.5f + Vector2.up * 0.3f;

        // Melakukan raycast untuk mencari player dalam jangkauan
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, direction, attackRange);

        // Iterasi semua objek yang terkena raycast
        foreach (RaycastHit2D hit in hits)
        {
            // Skip jika raycast mengenai enemy sendiri
            if (hit.collider.gameObject == gameObject) continue;

            // Jika menemukan player, berikan damage
            if (hit.collider.CompareTag("Player"))
            {
                Health playerHealth = hit.collider.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage); // Berikan damage sesuai variable
                }
                break; // Keluar dari loop setelah memberikan damage
            }
        }
    }

    // Flip sprite saat berganti arah
    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Visual debug untuk attack range
    private void OnDrawGizmos()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position + direction * 0.5f + Vector2.up * 0.3f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayStart, rayStart + direction * attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rayStart, 0.1f);
    }
}

