using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f;  // Cooldown antar serangan
    [SerializeField] private float attackRange = 2f;     // Jarak serangan ke depan
    [SerializeField] private int damage = 1;             // Damage yang diberikan
    [SerializeField] private float attackSoundDelay = 0.2f; // Delay untuk attack sound

    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private PlayerMovement playerMovement;
    private AudioSource attackAudioSource; // Audio Source untuk attack sound

    // Mobile input variable
    private bool mobileAttackPressed = false;


    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Mengambil Audio Source ketiga untuk attack sound
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 3)
        {
            attackAudioSource = audioSources[2]; // Audio Source ketiga untuk attack
        }
    }

    void Update()
    {
        // Gabungkan input PC dan mobile untuk attack
        bool attackInput = (Keyboard.current.lKey.wasPressedThisFrame || mobileAttackPressed)
                          && cooldownTimer > attackCooldown && playerMovement.canAttack();

        if (attackInput)
        {
            Attack();
            mobileAttackPressed = false; // Reset setelah attack
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        // Mainkan attack sound dengan delay
        if (attackAudioSource != null)
        {
            Invoke("PlayAttackSound", attackSoundDelay);
        }

        DamageEnemy();
    }

    // Method untuk memutar attack sound (dipanggil dengan delay)
    private void PlayAttackSound()
    {
        if (attackAudioSource != null)
        {
            attackAudioSource.Play();
        }
    }

    // Raycast ke depan untuk mencari enemy dan berikan damage
    private void DamageEnemy()
    {
        // Tentukan arah berdasarkan flip sprite player
        Vector2 direction = GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;

        // Raycast dimulai dari depan player untuk menghindari self-detection
        Vector2 rayStart = (Vector2)transform.position + direction * 0.5f + Vector2.up * 0.5f;

        // Cari enemy dengan raycast
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, attackRange);

        // Jika mengenai enemy, berikan damage
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    // Visual debug untuk attack range
    private void OnDrawGizmos()
    {
        Vector2 direction = GetComponent<SpriteRenderer>() != null && GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;
        Vector2 rayStart = (Vector2)transform.position + direction * 0.5f + Vector2.up * 0.5f;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rayStart, rayStart + direction * attackRange);
    }

    // Mobile input method untuk attack button
    public void OnMobileAttackPressed()
    {
        if (cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            mobileAttackPressed = true;
        }
    }
}
