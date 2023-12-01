using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public bool _isRootState = false;
    private PlayerState _currentSuperState;
    private PlayerState _currentSubState;
    protected Player player;
    protected PlayerStateMachine playerStateMachine;

    // Constructor to pass in a reference to the enemy its attached to and its stateMachine code
    public PlayerState(Player player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }

    // Virtual methods are used when we want to override a certain behavior for the dervied class
    
    // Execute code within method upon entering the state
    public abstract void EnterState();

    // Execute code within method upon exiting the state
    public abstract void ExitState();

    // Execute Update() code within method
    public abstract void FrameUpdate();

    // Execute FixedUpdate() code within method
    public abstract void PhysicsUpdate();

    // Check if its time to switch states, returns true if possible, false otherwise
    public abstract bool CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() {

        FrameUpdate();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void UpdatePhysicsStates()
    {
        PhysicsUpdate();
        if (_currentSubState != null)
        {
            _currentSubState.UpdatePhysicsStates();
        }
    }

    protected void SetSuperState(PlayerState newSuperState) {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

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
