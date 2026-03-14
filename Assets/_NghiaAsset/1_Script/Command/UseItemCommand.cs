using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using MyRule;

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

            // 1. Chạy Animation sử dụng vật phẩm
            user.animator.Play("Drinking");
            yield return null; // Đợi 1 frame để Animator cập nhật trạng thái

            // 2. Đợi đến thời điểm "uống" xong (0.8 giây) để kích hoạt hiệu ứng
            yield return new WaitForSeconds(0.8f);

            // 3. Xử lý Hiệu ứng hình ảnh (VFX)
            if (item.effect != null)
            {
                Flyweight_TB vfxInstance = FlyweightFactory_TB.Spawn(item.effect);

                if (vfxInstance != null)
                {
                    vfxInstance.transform.position = target.buffEffectSpawnPoint.position;
                    vfxInstance.transform.rotation = Quaternion.identity;
                    Debug.Log($"[ITEM] Đã spawn hiệu ứng: {item.effect.type} lên {target.name}");

                    // Trả VFX về Pool sau 2 giây
                    ReturnVFXTask(vfxInstance, 2000);
                }
            }

            // 4. Kích hoạt logic Buff/Hồi phục thông qua BuffManager đã cập nhật
            CharacterIItemBuffManager buffMgr = target.GetComponent<CharacterIItemBuffManager>();
            if (buffMgr != null)
            {
                // Thời gian duy trì mặc định là 3 lượt (bạn có thể đưa biến này vào Tb_Item nếu muốn)
                buffMgr.ApplyItemEffect(item, 3);
            }
            else
            {
                // Fallback nếu target không có BuffManager (chỉ xử lý hồi phục cơ bản)
                if (item.type == ItemType.HealthPotion) target.Heal(item.value);
                else if (item.type == ItemType.ManaPotion) target.RestoreMana(item.value);
            }

            // 5. Trừ số lượng và cập nhật UI
            item.quantity--;
            Debug.Log($"Vật phẩm {item.itemName} còn lại: {item.quantity}");

            if (item.quantity <= 0)
            {
                user.item.Remove(item);
            }

            // Cập nhật giao diện
            target.UpdateOwnUI();
            if (user.battleManager != null && user.battleUIManager != null)
            {
                user.battleUIManager.UpdateCharacterUI(target);
            }

            // 6. Đợi Animation kết thúc
            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Drinking"))
            {
                float normalizedTime = stateInfo.normalizedTime % 1f;
                float timeLeft = stateInfo.length * (1f - normalizedTime);

                if (timeLeft > 0) yield return new WaitForSeconds(timeLeft);
            }

            user.animator.Play("Idle");
            yield return new WaitForSeconds(0.5f);

            // 7. Kết thúc lượt
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