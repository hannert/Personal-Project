/// <summary>
/// The Player base state class for which concrete classes build upon
/// </summary>
public abstract class KinematicPlayerState
{
    /// <summary>
    /// boolean flag to denote if the state is the top most level in the hierarchy
    /// </summary>
    public bool _isRootState = false;

    /// <summary>
    /// reference to a parent state (if any)
    /// root states will typically NOT have a superstate
    /// </summary>
    private KinematicPlayerState _currentSuperState;

    /// <summary>
    /// reference to a child state
    /// </summary>
    private KinematicPlayerState _currentSubState;

    /// <summary>
    /// reference to the Player script
    /// <para>Player player</para>
    /// </summary>
    protected KinematicPlayer player;

    /// <summary>
    /// reference to the PlayerStateMachine
    /// <para>PlayerStateMachine playerStateMachine</para>
    /// </summary>
    protected KinematicPlayerStateMachine _psm;

    /// <summary>
    /// Constructor to pass in a reference to the player its attached to and its stateMachine code
    /// </summary>
    /// <param name="player"></param>
    /// <param name="playerStateMachine"></param>
    public KinematicPlayerState(KinematicPlayer player, KinematicPlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this._psm = playerStateMachine;
    }

    // Virtual methods are used when we want to override a certain behavior for the dervied class

    /// <summary>
    /// Execute code within method upon entering the state
    /// </summary>
    public virtual void EnterState() { }

    /// <summary>
    /// Execute code within method upon exiting the state
    /// </summary>
    public virtual void ExitState() { }

    /// <summary>
    /// Execute Update() code within method
    /// </summary>
    public virtual void FrameUpdate() { }

    /// <summary>
    /// Execute FixedUpdate() code within method
    /// </summary>
    public virtual void PhysicsUpdate() { }

    /// <summary>
    /// Check if its time to switch states, returns true if possible, false otherwise
    /// </summary>
    /// <returns>should return true if state change if successful, false if not</returns>
    public virtual bool CheckSwitchStates() { return false; }

    /// <summary>
    /// Sets up the correct substates for a parent state
    /// </summary>
    public virtual void InitializeSubState() { }

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
    protected void SetSuperState(KinematicPlayerState newSuperState) {
        _currentSuperState = newSuperState;
    }

    /// <summary>
    /// gives the current state a child that is the newSubState and transfers superstate status over to the newSubState
    /// </summary>
    /// <param name="newSubState"></param>
    protected void SetSubState(KinematicPlayerState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        newSubState.EnterState();
    }

    /// <summary>
    /// Switches the state of the state machine. 
    /// <para>If called within root state, the state machine will change its current state</para>
    /// <para>If called within a substate, the current superstate will set its substate to the newState</para>
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(KinematicPlayerState newState)
    {
        if (_isRootState)
        {
            // Root state is entered within the playerStateMachine
            _psm.ChangeState(newState);
        }
        else if (_currentSuperState != null)
        {
            // Old substate needs to be exited and new substate to be entered
            _currentSuperState._currentSubState.ExitState();
            _currentSuperState.SetSubState(newState);
            

        }
    }

}
