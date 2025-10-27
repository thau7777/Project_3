using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class RumbleManager : MonoBehaviour
    {
        private Gamepad _pad;
        private Coroutine _rumbleCoroutine;

        private void Start()
        {
            StartRumble();
        }

        async void StartRumble()
        {
            //await AsyncUtils.WaitForSeconds(1f);
            //RumblePulse(0.5f, 0.5f, 1.4f);
        }    

        public void RumbleOnStart()
        {
            RumblePulse(0.5f, 0.5f, 1.4f);
        }    

        public void RumblePulse(float lowFrequency, float highFrequency, float duration)
        {
            _pad = Gamepad.current;

            if (_pad == null)
                return;

            _pad.SetMotorSpeeds(lowFrequency, highFrequency);

            _rumbleCoroutine = StartCoroutine(StopRumble(duration, _pad));
        }
        
        private IEnumerator StopRumble(float duration, Gamepad pad)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            pad.SetMotorSpeeds(0f, 0f);
        }
    }
}
