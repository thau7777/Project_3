using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using MyRule;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Turnbase
{
    public class ParryCommand : ICommand
    {
        private Character character;
        private Character attacker;
        private CancellationTokenSource cts;
        public ParryCommand(Character character, Character attacker = null)
        {
            this.character = character;
            this.attacker = attacker;
        }

        public IEnumerator Execute()
        {
            string parryMsg = "PARRIED";
            bool isPerfect = (attacker != null && attacker.parryMissCount == 0 && attacker.isLastHit);

            if (!isPerfect && attacker != null) {
                Debug.Log($"<color=yellow>[PARRY DEBUG]</color> MissCount: {attacker.parryMissCount}, isLastHit: {attacker.isLastHit}");
            }

            
            cts?.Cancel();
            cts?.Dispose();
            cts= new CancellationTokenSource();

            Transition.TransitionValue(
                setter:value => Time.timeScale = value,
                from : 1 ,
                to: 0.8f,
                duration: 0.1f,
                cts.Token).Forget();
            

            if (isPerfect)
            {
                parryMsg = "Perfect PARRIED";
                character.animator.Play("Parry 2");
                CameraAction.instance.PerfectParryCamera(character);

                yield return new WaitForSeconds(0.2f);

                Transition.TransitionValue(
                setter: value => Time.timeScale = value,
                from: 0.8f,
                to: 0.1f,
                duration: 0.05f,
                cts.Token).Forget();

                yield return new WaitForSeconds(0.055f);
                Transition.TransitionValue(
                setter: value => Time.timeScale = value,
                from: 0.1f,
                to: 1f,
                duration: 0.3f,
                cts.Token).Forget();

            }
            else
            {
                character.animator.Play("Parry");
                CameraAction.instance.ParryCamera(character);
            }


            ParryPopup parryPopupComponent = character.GetComponent<ParryPopup>();
            parryPopupComponent.ShowParryPopup(character, parryMsg);


            CameraShaker.Instance.ShakeByDirection(new Vector3(0f, 0f, 1f), CinemachineImpulseDefinition.ImpulseShapes.Bump, 0.2f);

            SpawnEffectParry();

            yield return new WaitForSeconds(0.5f);

            character.animator.Play("Idle");

            character.stateMachine.SwitchState(character.stateMachine.waitingState);

            Debug.Log($"{character.name} kết thúc Parry.");

        }

        public void SpawnEffectParry()
        {
            OneShotVFXSettings_TB settings = Resources.Load<OneShotVFXSettings_TB>("Projectiles/Parry");

            if (settings != null)
            {
                Flyweight_TB effect = FlyweightFactory_TB.Spawn(settings);

                if (effect != null)
                {
                    effect.transform.SetParent(character.transform);
                    effect.transform.localPosition = Vector3.zero;
                    effect.transform.localRotation = Quaternion.identity;

                    effect.Initialize(character.transform.position, character.transform.rotation);
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy OneShotVFXSettings_TB tại Resources/Projectiles/Parry");
            }
        }
    }

}
