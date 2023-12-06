/// <summary>
/// The Player base state class for which concrete classes build upon
/// </summary>
public abstract class PlayerState
{
    /// <summary>
    /// boolean flag to denote if the state is the top most level in the hierarchy
    /// </summary>
    public bool _isRootState = false;

    /// <summary>
    /// reference to a parent state (if any)
    /// root states will typically NOT have a superstate
    /// </summary>
    private PlayerState _currentSuperState;

    /// <summary>
    /// reference to a child state
    /// </summary>
    private PlayerState _currentSubState;

    /// <summary>
    /// reference to the Player script
    /// <para>Player player</para>
    /// </summary>
    protected Player player;

    /// <summary>
    /// reference to the PlayerStateMachine
    /// <para>PlayerStateMachine playerStateMachine</para>
    /// </summary>
    protected PlayerStateMachine playerStateMachine;

    /// <summary>
    /// Constructor to pass in a reference to the player its attached to and its stateMachine code
    /// </summary>
    /// <param name="player"></param>
    /// <param name="playerStateMachine"></param>
    public PlayerState(Player player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }

    // Virtual methods are used when we want to override a certain behavior for the dervied class
    
    /// <summary>
    /// Execute code within method upon entering the state
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Execute code within method upon exiting the state
    /// </summary>
    public abstract void ExitState();

    /// <summary>
    /// Execute Update() code within method
    /// </summary>
    public abstract void FrameUpdate();

    /// <summary>
    /// Execute FixedUpdate() code within method
    /// </summary>
    public abstract void PhysicsUpdate();

    /// <summary>
    /// Check if its time to switch states, returns true if possible, false otherwise
    /// </summary>
    /// <returns>should return true if state change if successful, false if not</returns>
    public abstract bool CheckSwitchStates();

    /// <summary>
    /// Sets up the correct substates for a parent state
    /// </summary>
    public abstract void InitializeSubState();

    /// <summary>
    /// Updates the current states through its FrameUpdate() method and proceeds to its children to do the same
    /// </summary>
    public void UpdateStates() {

        FrameUpdate();
        _currentSubState?.UpdateStates();
    }

    /// <summary>
    /// Updates the current states through its PhysicsUpdate() method and proceeds to its children to do the same
    /// </summary>
    public void UpdatePhysicsStates()
    {
        PhysicsUpdate();
        _currentSubState?.UpdatePhysicsStates();
    }
    
    /// <summary>
    /// Sets the new superstate of the current state
    /// </summary>
    /// <param name="newSuperState"></param>
    protected void SetSuperState(PlayerState newSuperState) {
        _currentSuperState = newSuperState;
    }

    /// <summary>
    /// gives the current state a child that is the newSubState and transfers superstate status over to the newSubState
    /// </summary>
    /// <param name="newSubState"></param>
    protected void SetSubState(PlayerState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

    /// <summary>
    /// Switches the state of the state machine. 
    /// <para>If called within root state, the state machine will change its current state</para>
    /// <para>If called within a substate, the current superstate will set its substate to the newState</para>
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(PlayerState newState)
    {
        if (_isRootState)
        {
            playerStateMachine.ChangeState(newState);
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }

}
