using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


namespace Turnbase
{
    public class ParryCommand : ICommand
    {
        private Character character;

        public ParryCommand(Character character)
        {
            this.character = character;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{character.name} bắt đầu Parry!");

            CameraAction.instance.ParryCamera(character);

            character.animator.Play("Parry");

            Time.timeScale = 0.5f;


            ParryPopup parryPopupComponent = character.GetComponent<ParryPopup>();
            parryPopupComponent.ShowParryPopup(character);


            CameraShaker.Instance.ShakeByDirection(new Vector3(0.2f, 0.2f, 1.5f), CinemachineImpulseDefinition.ImpulseShapes.Bump, 0.2f);

            SpawnEffectParry();

            yield return new WaitForSeconds(0.8f);

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
