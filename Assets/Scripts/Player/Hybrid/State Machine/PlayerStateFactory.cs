
public class PlayerStateFactory
{
    private PlayerStateMachine _ctx {get; set;}


    public PlayerStateFactory(PlayerStateMachine psm){
        _ctx = psm;
    }


    # region Root States
    public PlayerState Grounded(){
        return new PlayerGroundedState(this, _ctx, "Grounded");
    }
    public PlayerState Airborne(){
        return new PlayerAirState(this, _ctx, "Airborne");
    }

    #endregion


    #region Movement States
    public PlayerState Idle(){
        return new PlayerIdleState(this, _ctx, "Idle");
    }
    
    public PlayerState Walking(){
        return new PlayerWalkingState(this, _ctx, "Walking");
    }

    public PlayerState Sprinting(){
        return  new PlayerSprintingState(this, _ctx, "Sprinting");
    }

    public PlayerState Crouching(){
        return new PlayerCrouchState(this, _ctx, "Crouch");
    }

    public PlayerState Sliding(){
        return new PlayerSlidingState(this, _ctx, "Sliding");
    }

    public PlayerState Rolling(){
        return new PlayerRollingState(this, _ctx, "Rolling");
    }


    
    



    #endregion

}
