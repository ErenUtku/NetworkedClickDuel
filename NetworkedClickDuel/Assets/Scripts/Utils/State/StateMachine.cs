using GameState;

namespace Utils.State
{
    public class StateMachine
    {
        public BaseState currentState;

        public void Initialize()
        {
            currentState = new LoadingState();
            currentState.EnterState();
        }

        public void Update()
        {
            currentState.UpdateState();
        }
        
        public void ChangeState(BaseState newState)
        {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }
    }
}