using UnityEngine;
using UnityEngine.InputSystem;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Input
{
    public sealed class KeyRepeatInteraction : IInputInteraction
    {
        public float RepeatDelay = 0.5f;
        public float RepeatInterval = 0.1f;
        public float PressPoint;
        private double _nextRepeatTime;


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Initialize()
        {
            InputSystem.RegisterInteraction<KeyRepeatInteraction>();
        }

        void IInputInteraction.Process(ref InputInteractionContext context)
        {
            if (RepeatDelay <= 0 || RepeatInterval <= 0)
            {
                Debug.LogError($"{nameof(RepeatDelay)} and {nameof(RepeatInterval)} must be greater than 0.");
                return;
            }

            if (context.timerHasExpired)
            {
                if (context.time >= _nextRepeatTime)
                {
                    _nextRepeatTime = context.time + RepeatInterval;
                    context.PerformedAndStayPerformed();
                    context.SetTimeout(RepeatInterval);
                }

                return;
            }

            switch (context.phase)
            {
                case InputActionPhase.Waiting:
                    if (context.ControlIsActuated(GetPressPoint()))
                    {
                        context.Started();
                        context.PerformedAndStayPerformed();
                        _nextRepeatTime = context.time + RepeatDelay;
                        context.SetTimeout(RepeatDelay);
                    }

                    break;

                case InputActionPhase.Performed:
                    if (!context.ControlIsActuated(GetReleasePoint()))
                    {
                        context.Canceled();
                    }

                    break;
            }
        }

        void IInputInteraction.Reset()
        {
            _nextRepeatTime = 0;
        }

        private float GetPressPoint()
        {
            if (PressPoint <= 0)
            {
                return InputSystem.settings.defaultButtonPressPoint;
            }

            return PressPoint;
        }

        private float GetReleasePoint()
        {
            if (PressPoint <= 0)
            {
                return InputSystem.settings.buttonReleaseThreshold;
            }

            return PressPoint;
        }
    }
}
