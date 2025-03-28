using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText; // Referência ao TextMeshProUGUI para mostrar a vida

    private PlayerController playerController;

    void Start()
    {
        // Encontre o PlayerController na cena
        playerController = FindObjectOfType<PlayerController>();

        // Verifique se o healthText foi atribuído no Inspector
        if (healthText == null)
        {
            Debug.LogError("Health Text não está atribuído no Inspector!");
        }
    }

    void Update()
    {
        if (playerController != null && healthText != null)
        {
            // Atualize o texto com a vida do jogador
            healthText.text = "Vida: " + playerController.GetCurrentHealth().ToString();
        }
    }
}
