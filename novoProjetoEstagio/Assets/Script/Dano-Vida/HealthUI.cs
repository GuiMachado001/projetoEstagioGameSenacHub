using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private PlayerController playerController;

    void Start()
    {

        playerController = FindObjectOfType<PlayerController>();


        if (healthText == null)
        {
            Debug.LogError("Health Text não está atribuído no Inspector!");
        }
    }

    void Update()
    {
        if (playerController != null && healthText != null)
        {

            healthText.text = "Vida: " + playerController.GetCurrentHealth().ToString();
        }
    }
}
