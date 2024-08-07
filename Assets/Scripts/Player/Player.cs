using System.Runtime.InteropServices;
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


    // TODO: create combat system to deal with damage and health
    public void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Debug.Log("Player health 0");
    }

    [field: SerializeField]
    public GameObject HandObject { get; private set; }

    [field: SerializeField]
    public GameObject StartingWeapon { get; private set; }

    private GameObject CurrentWeapon;
    void EquipWeapon(){
        CurrentWeapon = Instantiate(StartingWeapon, HandObject.transform);
        StateMachine.SetCurrentWeapon(CurrentWeapon);
    }

    void SwitchWeapon(GameObject newWeapon) {
        CurrentWeapon = newWeapon;
        StateMachine.SetCurrentWeapon(newWeapon);
    }

    #region State Machine

    [field: SerializeField]
    public PlayerStateMachine StateMachine { get; private set;}

    public PlayerStateFactory Psf { get; private set; } 


    #endregion

    public void Awake()
    {
        
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

    }

    void FixedUpdate()
    {        

    }
}
