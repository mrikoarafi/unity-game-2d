using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AskingSimmple : MonoBehaviour
{
    [Header("Quiz Settings")]
    [SerializeField] private GameObject questionPanel; // Panel UI untuk pertanyaan
    [SerializeField] private TextMeshProUGUI questionText; // Text Mesh Pro untuk pertanyaan
    [SerializeField] private Button[] answerButtons; // Array 4 button (1,2,3,4)
    [SerializeField] private float triggerRange = 1f; // Range untuk trigger quiz
    [SerializeField] private float wrongAnswerDamage = 1f; // Damage jika salah jawab

    [Header("Quiz Content")]
    [SerializeField] private string questionTextContent; // Teks pertanyaan
    [SerializeField] private string[] answerTexts = new string[4]; // Teks jawaban untuk 4 button
    [SerializeField] private int correctAnswerIndex = 0; // Index jawaban benar (0=A, 1=B, 2=C, 3=D)

    private bool questionActive = false; // Status quiz aktif
    private PlayerMovement playerMovement; // Reference ke player movement
    private int correctAnswer; // Index jawaban yang benar (0-3)
    private Animator playerAnimator; // Reference ke player animator
    private Rigidbody2D playerRb; // Reference ke player rigidbody
    private bool questionCompleted = false; // Status quiz sudah selesai dengan benar
    private Image lastWrongButton;
    private Color lastOriginalColor;
    private AudioSource correctAudioSource; // Audio Source untuk suara jawaban benar

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Mengambil Audio Source pertama untuk suara jawaban benar
        AudioSource[] audioSources = GetComponents<AudioSource>();

        correctAudioSource = audioSources[0]; // Audio Source pertama (index 0)
    }

    // Ketika player masuk ke trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !questionActive && !questionCompleted)
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerAnimator = collision.GetComponent<Animator>();
            playerRb = collision.GetComponent<Rigidbody2D>();
            ShowQuestion();
        }
    }

    // Tampilkan pertanyaan
    private void ShowQuestion()
    {
        questionActive = true;

        // Disable player movement - biarkan PlayerMovement handle idle
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Generate pertanyaan dan jawaban
        GenerateQuestion();

        // Tampilkan panel pertanyaan
        if (questionPanel != null)
            questionPanel.SetActive(true);
    }

    // Generate pertanyaan dari variable
    private void GenerateQuestion()
    {
        // Set text pertanyaan dari variable
        if (questionText != null)
            questionText.text = questionTextContent;

        // Jawaban benar dari variable
        correctAnswer = correctAnswerIndex;

        // Set text pada button dari variable
        for (int i = 0; i < answerButtons.Length && i < answerTexts.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                // Set text pada button dari array
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answerTexts[i];

                // Remove listener lama dan tambah yang baru
                answerButtons[i].onClick.RemoveAllListeners();
                int buttonIndex = i;
                answerButtons[i].onClick.AddListener(() => CheckAnswer(buttonIndex));
            }
        }
    }

    // Cek jawaban player
    public void CheckAnswer(int selectedButtonIndex)
    {
        if (selectedButtonIndex == correctAnswer)
        {
            // Mainkan suara jawaban benar
            correctAudioSource.Play();

            // Jawaban benar - tandai selesai dan lanjut game
            questionCompleted = true;
            EndQuiz(true);

            // Enable player movement kembali
            if (playerMovement != null)
                playerMovement.enabled = true;
        }
        else
        {
            // Jawaban salah - berikan damage dan efek berkedip
            if (playerMovement != null)
            {
                Health playerHealth = playerMovement.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(wrongAnswerDamage);
                }
            }

            // Efek berkedip merah pada button yang salah
            BlinkWrongButton(selectedButtonIndex);

            // TIDAK memanggil EndQuiz - pertanyaan tetap tampil
            // Enable player movement agar bisa bergerak sedikit
            if (playerMovement != null)
                playerMovement.enabled = true;
        }
    }

    // Efek berkedip merah pada button yang salah
    private void BlinkWrongButton(int buttonIndex)
    {
        if (buttonIndex < answerButtons.Length && answerButtons[buttonIndex] != null)
        {
            Button wrongButton = answerButtons[buttonIndex];
            Image buttonImage = wrongButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                // Simpan warna asli
                Color originalColor = buttonImage.color;

                // Set warna merah dengan opacity
                buttonImage.color = new Color(1f, 0f, 0f, 0.7f);

                // Kembalikan ke warna asli setelah 0.5 detik
                Invoke("RestoreButtonColor", 0.5f);

                // Simpan reference untuk restore
                lastWrongButton = buttonImage;
                lastOriginalColor = originalColor;
            }
        }
    }

    // Kembalikan warna button ke semula
    private void RestoreButtonColor()
    {
        if (lastWrongButton != null)
        {
            lastWrongButton.color = lastOriginalColor;
        }
    }

    // Akhiri quiz
    private void EndQuiz(bool correct)
    {
        questionActive = false;

        // Sembunyikan panel
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    // Visual debug untuk trigger range
    private void OnDrawGizmos()
    {
        // Gambar lingkaran untuk menunjukkan trigger range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRange);

        // Gambar area dengan transparansi
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, triggerRange);
    }
}
