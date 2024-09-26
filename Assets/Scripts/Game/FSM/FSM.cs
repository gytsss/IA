using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.FSM
{
    public class FSM<EnumState, EnumFlag>
        where EnumState : Enum
        where EnumFlag : Enum
    {
        private const int UNNASIGNED_TRANSITION = -1;
        private int stateAmount = 0;
        private int currentState = 0;
        private Dictionary<int, State> behaviours;
        private Dictionary<int, Func<object[]>> _behaviourTickParameters;
        private Dictionary<int, Func<object[]>> _behaviourOnEnterParameters;
        private Dictionary<int, Func<object[]>> _behaviourOnExitParameters;

        private (int destinationState, Action onTransition)[,] transitions;

        ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

        private BehavioursActions GetCurrentStateOnEnterBehaviours => behaviours[currentState]
            .GetOnEnterBehaviour(_behaviourOnEnterParameters[currentState]?.Invoke());

        private BehavioursActions GetCurrentStateOnExitBehaviours => behaviours[currentState]
            .GetOnExitBehaviour(_behaviourOnExitParameters[currentState]?.Invoke());

        private BehavioursActions GetCurrentStateTickBehaviours => behaviours[currentState]
            .GetTickBehaviour(_behaviourTickParameters[currentState]?.Invoke());

        public FSM()
        {
            int statesAmount = Enum.GetValues(typeof(EnumState)).Length;
            int flags = Enum.GetValues(typeof(EnumFlag)).Length;

            behaviours = new Dictionary<int, State>();
            transitions = new (int, Action)[statesAmount, flags];

            for (int i = 0; i < statesAmount; i++)
            {
                for (int j = 0; j < flags; j++)
                {
                    transitions[i, j] = (UNNASIGNED_TRANSITION, null);
                }
            }

            _behaviourTickParameters = new Dictionary<int, Func<object[]>>();
            _behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
            _behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
        }

        public void AddBehaviour<T>(EnumState state, Func<object[]> onTickParameters = null,
            Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
        {
            int stateIndex = Convert.ToInt32(state);
            if (behaviours.ContainsKey(stateIndex)) return;

            State newBehaviour = new T();
            newBehaviour.OnFlag += Transition;
            behaviours.Add(stateIndex, newBehaviour);
            _behaviourTickParameters.Add(stateIndex, onTickParameters);
            _behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            _behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }

        public void ForceState(EnumState state)
        {
            currentState = Convert.ToInt32(state);
        }

        public void SetTransition(EnumState originState, EnumFlag flag, EnumState destinationState,
            Action onTransition = null)
        {
            transitions[Convert.ToInt32(originState), Convert.ToInt32(flag)] =
                (Convert.ToInt32(destinationState), onTransition);
        }

        private void Transition(Enum flag)
        {
            if (transitions[currentState, Convert.ToInt32(flag)].destinationState != UNNASIGNED_TRANSITION)
            {
                ExecuteBehaviour(GetCurrentStateOnExitBehaviours);

                transitions[currentState, Convert.ToInt32(flag)].onTransition?.Invoke();

                currentState = transitions[currentState, Convert.ToInt32(flag)].destinationState;

                ExecuteBehaviour(GetCurrentStateOnEnterBehaviours);
            }
        }


        public void Tick()
        {
            if (behaviours.ContainsKey(currentState))
            {
                ExecuteBehaviour(GetCurrentStateTickBehaviours);
            }
        }

        private void ExecuteBehaviour(BehavioursActions behavioursActions)
        {
            if (behavioursActions.Equals(default(BehavioursActions)))
                return;

            int executionOrder = 0;

            while ((behavioursActions.MainThreadBehaviours != null && behavioursActions.MainThreadBehaviours.Count > 0) ||
                   (behavioursActions.MultithreadblesBehaviours != null && behavioursActions.MultithreadblesBehaviours.Count > 0))
            {
                Task multithreadableBehaviour = new Task(() =>
                {
                    if (behavioursActions.MultithreadblesBehaviours != null)
                    {
                        if (behavioursActions.MultithreadblesBehaviours.ContainsKey(executionOrder))
                        {
                            Parallel.ForEach(behavioursActions.MultithreadblesBehaviours[executionOrder],
                                parallelOptions,
                                (behaviour) => { behaviour?.Invoke(); });
                            behavioursActions.MultithreadblesBehaviours.TryRemove(executionOrder,
                                out _); //out = no me importa lo que devuelve
                        }
                    }
                });

                multithreadableBehaviour.Start();

                if (behavioursActions.MainThreadBehaviours != null)
                {
                    if (behavioursActions.MainThreadBehaviours.ContainsKey(executionOrder))
                    {
                        foreach (Action behaviour in behavioursActions.MainThreadBehaviours[executionOrder])
                        {
                            behaviour?.Invoke();
                        }

                        behavioursActions.MainThreadBehaviours.Remove(executionOrder);
                    }
                }

                multithreadableBehaviour.Wait();
                executionOrder++;
            }

            behavioursActions.TransitionBehavior?.Invoke();
        }
    }
}