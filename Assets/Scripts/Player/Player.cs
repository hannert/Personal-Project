using UnityEditor;
using UnityEngine;

/// <summary>
/// The overarching Player script to be attached to the player GameObject
/// <para>Contains everything about the player..</para>
/// </summary>
public class Player : MonoBehaviour, IDamagable
{
    public float maxHealth { get ; set ; }
    public float currentHealth { get; set; }

    [Header("Movement")]
    
    [Tooltip("The players current speed")]
    public float currentSpeed;

    [Tooltip("The players maximum speed")]
    public float speed = 10.0f;

    [Tooltip("Curve for acceleration multiplier when turning the character in opposing directions")]   
    public AnimationCurve AccelerationMultiplier;

    [Tooltip("Sprint force modifier")]
    // TODO: Change this to a modifier rather than a set speed for sprint ?
    public float sprintSpeed = 15.0f;

    [Tooltip("Force added when player jumps")]
    public float jumpForce = 20.0f;
    

    // Internal Data for the player
    private Rigidbody playerRb;
    private CapsuleCollider playerCap;
    private new CameraController camera;
    private Animator playerAnim;
    

    // TODO: create combat system to deal with damage and health
    public void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Debug.Log("Player health 0");
    }


    public GameObject handObject;
    public GameObject startingWeapon;

    private GameObject currentWeapon;
    void EquipWeapon(){
        GameObject tempWeapon = Instantiate(startingWeapon, handObject.transform);
    }

    void SwitchWeapon(GameObject newWeapon) {
        currentWeapon = newWeapon;
        stateMachine.setCurrentWeapon(newWeapon);
    }

    #region State Machine

    public PlayerStateMachine stateMachine;

    protected PlayerStateFactory _psf; 


    #endregion

    public void Awake()
    {
        EquipWeapon();
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();
        playerAnim = GetComponentInChildren<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();

        stateMachine = new PlayerStateMachine(playerRb, playerCap, camera, playerAnim, AccelerationMultiplier);
        _psf = new PlayerStateFactory(stateMachine);

        stateMachine.Initialize(_psf.Grounded());
    }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;


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
        currentSpeed = stateMachine.currentSpeed;
        stateMachine.UpdatePhysicsStates();
    }
}
