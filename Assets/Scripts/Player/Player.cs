using deVoid.Utils;
using Unity.VisualScripting;
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
    
    #region Animation
    private Animator PlayerAnim { get; set; }
    
    public class DamageTick : ASignal {}

    public class AirborneAttack : ASignal {}
    public class AttackApplyForce : ASignal {}

    // TODO: Hook up animation events with player functions
    public void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void DealDamage() {
        // Send an attack signal to the state machine
        // -> From state machine to the weapon
        // -> -> Weapon should reset it's hitEntities list

        // Or send a signal directly to the weapon if available?
        Signals.Get<DamageTick>().Dispatch();



    }

    /// <summary>
    /// Send signal to attacking state to switch root state to airborne
    /// </summary>
    public void GoAirborne()
    {
        Signals.Get<AirborneAttack>().Dispatch();
    }

    /// <summary>
    /// Send signal to the attacking state to apply the force
    /// </summary>
    public void AttackForce()
    {
        Signals.Get<AttackApplyForce>().Dispatch();

    }


    #endregion

    public void Die()
    {
        Debug.Log("Player health 0");
    }

    [field: SerializeField]
    public GameObject WeaponParent { get; private set; }

    [field: SerializeField]
    public GameObject StartingWeapon { get; private set; }

    private GameObject CurrentWeapon;
    void EquipWeapon(){
        CurrentWeapon = Instantiate(StartingWeapon, WeaponParent.transform);
        StateMachine.SetCurrentWeapon(CurrentWeapon);
    }

    void SwitchWeapon(GameObject newWeapon) {
        CurrentWeapon = newWeapon;
        StateMachine.SetCurrentWeapon(newWeapon);
    }

    #region State Machine

    public PlayerStateMachine StateMachine { get; private set;}

    public PlayerStateFactory Psf { get; private set; } 


    #endregion

    public void Awake()
    {
        
        PlayerRb = GetComponent<Rigidbody>();
        PlayerCap = GetComponent<CapsuleCollider>();
        PlayerAnim = GetComponent<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        Camera = GameObject.Find("Camera").GetComponent<CameraController>();

        StateMachine = new PlayerStateMachine(PlayerRb, PlayerCap, Camera, PlayerAnim, AccelerationMultiplier);
        Psf = new PlayerStateFactory(StateMachine);
        EquipWeapon(); 
        StateMachine.Initialize(Psf.Grounded());
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;


    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
        // stateMachine.speed = speed;
        // stateMachine.jumpForce = jumpForce;

    }

    void FixedUpdate()
    {        
        // Expose the current speed of the player 
        CurrentSpeed = StateMachine.CurrentSpeed;
        StateMachine.UpdatePhysicsStates();
    }
}
