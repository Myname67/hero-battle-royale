using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Character controlledCharacter;
    private Camera mainCamera;
    
    public static InputManager Instance { get; private set; }
    
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
        mainCamera = Camera.main;
    }
    
    public void SetControlledCharacter(Character character)
    {
        controlledCharacter = character;
    }
    
    private void Update()
    {
        if (controlledCharacter == null || !controlledCharacter.IsAlive())
            return;
        
        HandleMovement();
        HandleAbilities();
    }
    
    private void HandleMovement()
    {
        Vector3 direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        
        if (direction != Vector3.zero)
        {
            controlledCharacter.Move(direction.normalized);
        }
    }
    
    private void HandleAbilities()
    {
        Vector3 targetDirection = GetMouseDirection();
        
        if (Input.GetMouseButtonDown(0))
            controlledCharacter.BasicAttack(targetDirection);
        
        if (Input.GetKeyDown(KeyCode.Q))
            controlledCharacter.UseAlpha(targetDirection);
        
        if (Input.GetKeyDown(KeyCode.E))
            controlledCharacter.UseBeta();
        
        if (Input.GetKeyDown(KeyCode.R))
            controlledCharacter.UseGamma();
        
        if (Input.GetKeyDown(KeyCode.Space))
            controlledCharacter.UseSpecial();
    }
    
    private Vector3 GetMouseDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return ray.direction.normalized;
    }
}
