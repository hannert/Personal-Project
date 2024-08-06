using UnityEditor;
using UnityEngine;

/// <summary>
/// The overarching Player script to be attached to the player GameObject
/// <para>Contains everything about the player..</para>
/// </summary>
/// 
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IDamagable
{
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    [Header("Movement")]
    
    [Tooltip("The players current speed")]
    public float CurrentSpeed;

    [Tooltip("The players maximum speed")]
    public float Speed = 10.0f;

    [Tooltip("Curve for acceleration multiplier when turning the character in opposing directions")]   
    public AnimationCurve AccelerationMultiplier;

    [Tooltip("Sprint force modifier")]
    // TODO: Change this to a modifier rather than a set speed for sprint ?
    public float SprintSpeed = 15.0f;

    [Tooltip("Force added when player jumps")]
    public float JumpForce = 20.0f;
    

    // Internal Data for the player
    private Rigidbody PlayerRb { get; set; }
    private CapsuleCollider PlayerCap { get; set; }
    private CameraController Camera { get; set; }
    private Animator PlayerAnim { get; set; }
    

    // TODO: create combat system to deal with damage and health
    public void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Debug.Log("Player health 0");
    }


    public GameObject HandObject { get; private set; }
    public GameObject StartingWeapon { get; private set; }

    private GameObject CurrentWeapon;
    void EquipWeapon(){
        CurrentWeapon = Instantiate(StartingWeapon, HandObject.transform);
        stateMachine.SetCurrentWeapon(CurrentWeapon);
    }

    void SwitchWeapon(GameObject newWeapon) {
        CurrentWeapon = newWeapon;
        stateMachine.SetCurrentWeapon(newWeapon);
    }

    #region State Machine

    public PlayerStateMachine stateMachine { get; private set;}

    public PlayerStateFactory _psf { get; private set; } 


    #endregion

    public void Awake()
    {
        
        PlayerRb = GetComponent<Rigidbody>();
        PlayerCap = GetComponent<CapsuleCollider>();
        PlayerAnim = GetComponent<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        Camera = GameObject.Find("Camera").GetComponent<CameraController>();

        stateMachine = new PlayerStateMachine(PlayerRb, PlayerCap, Camera, PlayerAnim, AccelerationMultiplier);
        _psf = new PlayerStateFactory(stateMachine);
        EquipWeapon(); 
        stateMachine.Initialize(_psf.Grounded());
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;


    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        // stateMachine.speed = speed;
        // stateMachine.jumpForce = jumpForce;

    }

    void FixedUpdate()
    {        
        // Expose the current speed of the player 
        CurrentSpeed = stateMachine.CurrentSpeed;
        stateMachine.UpdatePhysicsStates();
    }
}
