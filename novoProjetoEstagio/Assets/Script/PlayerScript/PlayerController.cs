using UnityEngine;
using TMPro;  // Para acessar o componente TextMeshProUGUI
using UnityEngine.SceneManagement;  // Para recarregar a cena

public class PlayerController : MonoBehaviour
{
    public float maxHealth = 5f;  // Vida máxima
    public float currentHealth;   // Vida atual
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 15f;
    public GameObject deathPanel;  // Referência ao painel do Canvas

    private Rigidbody rb;
    private bool isGrounded;

    private Animator animator;

    public TextMeshProUGUI vidaText;  // Referência ao componente TextMeshProUGUI
    public TextMeshProUGUI deathText; // Referência ao texto de "Você morreu"
    private float initialHealth;  // Variável para armazenar o valor inicial da vida máxima

    private bool isDead = false;  // Flag para verificar se o jogador morreu
    private float countdown = 3f; // Tempo para reiniciar o jogo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;

        rb.drag = 5f;
        rb.angularDrag = 10f;

        // Inicializando a vida e armazenando a vida máxima
        currentHealth = maxHealth;
        initialHealth = maxHealth;

        // Inicializando o texto
        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;  // Inicializa com o valor da vida
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(false);  // Desativa o texto de morte inicialmente
        }

        // Debugando para garantir que o valor de maxHealth está correto
        Debug.Log("Max Health: " + maxHealth);  
        Debug.Log("Current Health: " + currentHealth);
    }

    void Update()
    {
        // Se o jogador morreu, não faz mais nada (bloqueia o movimento)
        if (isDead)
        {
            countdown -= Time.deltaTime;
            if (deathText != null)
            {
                deathText.text = "Você morreu, Jogo reiniciando em " + Mathf.Ceil(countdown) + "...";
            }

            if (countdown <= 0f)
            {
                RestartGame();
            }

            return;
        }

        // Lógica de movimento do jogador
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        UpdateAnimationBlend(moveDirection, speed);

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 targetVelocity = moveDirection * speed;
            targetVelocity.y = rb.velocity.y;

            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 10f);

            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Verifica se a vida está cheia e exibe a mensagem
        if (currentHealth == initialHealth && vidaText != null)
        {
            vidaText.text = "Vida Cheia";
        }
        else if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;  // Exibe o valor da vida
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            Heal(1);
            Debug.Log("Vida aumentada!");
        }
        else if (other.CompareTag("DamagePickup"))
        {
            TakeDamage(1);
            Debug.Log("Vida reduzida!");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        // Atualizando o texto da vida após o dano
        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;  // Atualiza o texto com o valor atual da vida
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Atualizando o texto da vida após a cura
        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;
        }
    }

public void Die()
{
    isDead = true;  // Marca o jogador como morto
    rb.velocity = Vector3.zero;  // Para qualquer movimento do jogador

    // Verifica se o texto de morte está disponível e ativa ele
    if (deathText != null)
    {
        deathText.gameObject.SetActive(true);  // Ativa o texto de morte
        Debug.Log("Texto de Morte Ativado!");  // Debug para garantir que o texto foi ativado
    }
    else
    {
        Debug.Log("Texto de morte não encontrado!");  // Caso o texto de morte não seja encontrado
    }

    // Ativa o painel de morte
    if (deathPanel != null)
    {
        deathPanel.SetActive(true);  // Torna o painel visível
        Debug.Log("Painel de Morte Ativado!");  // Debug para garantir que o painel foi ativado
    }

    Debug.Log("Você morreu!");
}



    private void RestartGame()
    {
        // Aqui reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        deathPanel.SetActive(false);
    }

    private void UpdateAnimationBlend(Vector3 moveDirection, float speed)
    {
        if (animator != null)
        {
            float blendValue = moveDirection.magnitude * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) / runSpeed;
            animator.SetFloat("Blend", blendValue);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Método para retornar a vida atual (getter)
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
