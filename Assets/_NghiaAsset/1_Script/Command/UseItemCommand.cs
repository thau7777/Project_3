using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static Turnbase.Tb_Item;

namespace Turnbase
{
    public class UseItemCommand : ICommand
    {
        private Character user;
        private Character target;
        private Tb_Item item;

        public UseItemCommand(Character user, Character target, Tb_Item item)
        {
            this.user = user;
            this.target = target;
            this.item = item;
        }

        public IEnumerator Execute()
        {
            if (user == null || target == null || item == null) yield break;

            Debug.Log($"{user.name} đang sử dụng {item.itemName} lên {target.name}");

            user.animator.Play("Drinking");
            yield return null;

            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            float totalDuration = stateInfo.length;

            yield return new WaitForSeconds(0.8f);

            if (item.effect != null)
            {
                Flyweight_TB vfxInstance = FlyweightFactory_TB.Spawn(item.effect);

                if (vfxInstance != null)
                {
                    vfxInstance.transform.position = target.transform.position;
                    vfxInstance.transform.rotation = Quaternion.identity;
                    Debug.Log($"[ITEM] Đã spawn hiệu ứng: {item.effect.type} lên {target.name}");
                }

                ReturnVFXTask(vfxInstance, 2000);
            }


            CharacterIItemBuffManager buffMgr = target.GetComponent<CharacterIItemBuffManager>();
            if (buffMgr != null)
            {
                buffMgr.ApplyItemEffect(item, 3);
            }
            else
            {
                switch (item.type)
                {
                    case ItemType.Healing: target.Heal(item.value); break;
                    case ItemType.Mana: target.RestoreMana(item.value); break;
                }
            }

            item.quantity--;
            Debug.Log($"Vật phẩm {item.itemName} còn lại: {item.quantity}");

            if (item.quantity <= 0)
            {
                user.item.Remove(item);
            }

            target.UpdateOwnUI();
            if (user.battleManager != null)
            {
                user.battleUIManager.UpdateCharacterUI(target);
            }

            stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Drinking"))
            {
                float normalizedTime = stateInfo.normalizedTime % 1f;
                float timeLeft = stateInfo.length * (1f - normalizedTime);

                if (timeLeft > 0)
                {
                    yield return new WaitForSeconds(timeLeft);
                }
            }

            user.animator.Play("Idle");

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Sử dụng vật phẩm xong, kết thúc lượt.");
            user.battleManager.EndTurn(user);
        }

        private async void ReturnVFXTask(Flyweight_TB vfx, int delayMs)
        {
            await Task.Delay(delayMs);

            if (vfx != null)
            {
                FlyweightFactory_TB.ReturnToPool(vfx);
            }
        }
    }
}