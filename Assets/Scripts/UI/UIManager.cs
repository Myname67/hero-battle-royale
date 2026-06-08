using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI alphaAbilityText;
    [SerializeField] private TextMeshProUGUI betaAbilityText;
    [SerializeField] private TextMeshProUGUI gammaAbilityText;
    [SerializeField] private TextMeshProUGUI specialAbilityText;
    
    private Character playerCharacter;
    
    public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void SetPlayerCharacter(Character character)
    {
        playerCharacter = character;
        if (characterNameText != null)
            characterNameText.text = character.GetCharacterName();
    }
    
    private void Update()
    {
        if (playerCharacter == null) return;
        
        UpdateHealthUI();
        UpdateTimerUI();
        UpdateKillsUI();
    }
    
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {playerCharacter.GetCurrentHealth()} / {playerCharacter.GetMaxHealth()}";
        
        if (healthBar != null)
        {
            float healthPercent = (float)playerCharacter.GetCurrentHealth() / playerCharacter.GetMaxHealth();
            healthBar.fillAmount = healthPercent;
        }
    }
    
    private void UpdateTimerUI()
    {
        if (timerText != null && GameManager.Instance != null)
        {
            float timeRemaining = GameManager.Instance.GetMatchTimeRemaining();
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    private void UpdateKillsUI()
    {
        if (killsText != null)
            killsText.text = $"Kills: {playerCharacter.GetKillCount()}";
    }
}
