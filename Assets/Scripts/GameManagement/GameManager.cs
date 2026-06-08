using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int playersPerTeam = 5;
    [SerializeField] private float matchDuration = 600f; // 10 minutes
    [SerializeField] private GameObject kateroCharacterPrefab;
    [SerializeField] private GameObject mizunoCharacterPrefab;
    [SerializeField] private Transform team1SpawnPoint;
    [SerializeField] private Transform team2SpawnPoint;
    
    private List<Character> team1Characters = new List<Character>();
    private List<Character> team2Characters = new List<Character>();
    private bool matchInProgress = false;
    private float matchTimeRemaining;
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        InitializeMatch();
    }
    
    private void Update()
    {
        if (!matchInProgress) return;
        
        UpdateMatchTimer();
        CheckMatchEndConditions();
    }
    
    public void InitializeMatch()
    {
        matchInProgress = true;
        matchTimeRemaining = matchDuration;
        
        // Spawn team 1
        for (int i = 0; i < playersPerTeam; i++)
        {
            Character character = SpawnCharacter(i % 2 == 0 ? kateroCharacterPrefab : mizunoCharacterPrefab, team1SpawnPoint, 0);
            team1Characters.Add(character);
        }
        
        // Spawn team 2
        for (int i = 0; i < playersPerTeam; i++)
        {
            Character character = SpawnCharacter(i % 2 == 0 ? kateroCharacterPrefab : mizunoCharacterPrefab, team2SpawnPoint, 1);
            team2Characters.Add(character);
        }
        
        Debug.Log($"Match started! {playersPerTeam} vs {playersPerTeam}");
    }
    
    private Character SpawnCharacter(GameObject prefab, Transform spawnPoint, int teamId)
    {
        GameObject instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Character character = instance.GetComponent<Character>();
        character.SetTeamId(teamId);
        
        if (teamId == 0 && InputManager.Instance != null)
            InputManager.Instance.SetControlledCharacter(character);
        
        return character;
    }
    
    private void UpdateMatchTimer()
    {
        matchTimeRemaining -= Time.deltaTime;
        if (matchTimeRemaining <= 0)
        {
            matchTimeRemaining = 0;
        }
    }
    
    private void CheckMatchEndConditions()
    {
        // Check if one team is eliminated
        int team1Alive = 0, team2Alive = 0;
        
        foreach (Character character in team1Characters)
            if (character.IsAlive()) team1Alive++;
        
        foreach (Character character in team2Characters)
            if (character.IsAlive()) team2Alive++;
        
        if (team1Alive == 0 || team2Alive == 0)
        {
            EndMatch(team1Alive > 0 ? 0 : 1);
        }
        
        // Check if time ran out
        if (matchTimeRemaining <= 0)
        {
            int team1Kills = 0, team2Kills = 0;
            foreach (Character character in team1Characters) team1Kills += character.GetKillCount();
            foreach (Character character in team2Characters) team2Kills += character.GetKillCount();
            
            EndMatch(team1Kills > team2Kills ? 0 : 1);
        }
    }
    
    public void EndMatch(int winningTeam)
    {
        if (!matchInProgress) return;
        matchInProgress = false;
        
        Debug.Log($"Match ended! Team {winningTeam} wins!");
        
        // Play victory animations
        List<Character> winningTeam_chars = winningTeam == 0 ? team1Characters : team2Characters;
        foreach (Character character in winningTeam_chars)
        {
            if (character is Katero katero)
                katero.OnVictory();
        }
    }
    
    public bool IsMatchInProgress() => matchInProgress;
    public float GetMatchTimeRemaining() => matchTimeRemaining;
    public List<Character> GetTeam1() => team1Characters;
    public List<Character> GetTeam2() => team2Characters;
}
