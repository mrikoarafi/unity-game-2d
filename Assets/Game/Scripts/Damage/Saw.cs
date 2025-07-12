using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // Kecepatan rotasi dalam derajat per detik
    [SerializeField] private float damage = 1f; // Damage yang diberikan saw
    [SerializeField] private float moveDistance = 3f; // Jarak pergerakan saw
    [SerializeField] private float moveSpeed = 2f; // Kecepatan pergerakan saw

    private Vector3 startPosition; // Posisi awal saw
    private bool movingRight = true; // Arah pergerakan
    private float lastHitTime = 0f; // Waktu terakhir hit player
    private float hitCooldown = 1f; // Cooldown antar hit

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position; // Simpan posisi awal
    }

    // Update is called once per frame
    void Update()
    {
        // Rotasi saw terus menerus
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Pergerakan saw horizontal kanan-kiri
        MoveSaw();
    }

    private void MoveSaw()
    {
        if (movingRight)
        {
            // Ubah posisi X langsung tanpa terpengaruh rotasi
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= startPosition.x + moveDistance)
                movingRight = false;
        }
        else
        {
            // Ubah posisi X langsung tanpa terpengaruh rotasi
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= startPosition.x)
                movingRight = true;
        }
    }

    // Dipanggil saat objek lain menyentuh collider (bukan trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah objek yang menyentuh adalah Player
        if (collision.gameObject.tag == "Player")
        {
            // Cek cooldown untuk mencegah spam damage/knockback
            if (Time.time - lastHitTime < hitCooldown)
                return;

            lastHitTime = Time.time;

            // Berikan damage ke player
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);

            // Berikan efek knockback horizontal saja
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Hitung arah knockback horizontal saja (dari saw ke player)
                float direction = collision.transform.position.x - transform.position.x;

                // Tentukan arah mental: kanan jika positif, kiri jika negatif
                Vector2 knockbackDirection = direction > 0 ? Vector2.right : Vector2.left;

                // Terapkan force knockback horizontal - tidak reset velocity
                float knockbackForce = 15f;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
