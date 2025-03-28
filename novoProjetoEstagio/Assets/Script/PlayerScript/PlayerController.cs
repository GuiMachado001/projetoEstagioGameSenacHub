using UnityEngine;
using TMPro;  
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{
    public float maxHealth = 5f;  
    public float currentHealth;   
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 15f;
    public GameObject deathPanel;  

    private Rigidbody rb;
    private bool isGrounded;

    private Animator animator;

    public TextMeshProUGUI vidaText;  
    public TextMeshProUGUI deathText; 
    private float initialHealth;  

    private bool isDead = false;  
    private float countdown = 3f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;

        rb.drag = 5f;
        rb.angularDrag = 10f;


        currentHealth = maxHealth;
        initialHealth = maxHealth;

        // Inicializando o texto
        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;  
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(false); 
        }


        Debug.Log("Max Health: " + maxHealth);  
        Debug.Log("Current Health: " + currentHealth);
    }

    void Update()
    {

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


        if (currentHealth == initialHealth && vidaText != null)
        {
            vidaText.text = "Vida Cheia";
        }
        else if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth; 
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


        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;  
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }


        if (vidaText != null)
        {
            vidaText.text = "Vidas: " + currentHealth;
        }
    }

public void Die()
{
    isDead = true; 
    rb.velocity = Vector3.zero;  


    if (deathText != null)
    {
        deathText.gameObject.SetActive(true);  
        Debug.Log("Texto de Morte Ativado!");  
    }
    else
    {
        Debug.Log("Texto de morte não encontrado!");  
    }


    if (deathPanel != null)
    {
        deathPanel.SetActive(true); 
        Debug.Log("Painel de Morte Ativado!"); 
    }

    Debug.Log("Você morreu!");
}



    private void RestartGame()
    {

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


    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
