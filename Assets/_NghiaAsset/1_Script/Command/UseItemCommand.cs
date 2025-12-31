using System.Collections;
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


            yield return new WaitForSeconds(0.8f);

            switch (item.type)
            {
                case ItemType.Healing:
                    target.Heal(item.value);
                    break;

                case ItemType.Mana:
                    target.RestoreMana(item.value);
                    break;

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

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Sử dụng vật phẩm xong, kết thúc lượt.");
            user.battleManager.EndTurn(user);
        }
    }
}