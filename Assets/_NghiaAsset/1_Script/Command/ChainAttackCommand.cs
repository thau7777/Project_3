using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class ChainAttackCommand : SkillCommand
    {
        private BattleManager battleManager;
        private List<Character> chainTargets = new List<Character>();
        private LineRenderer lineRenderer;

        public ChainAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
          : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            // 1. Lấy danh sách mục tiêu
            chainTargets.Clear();
            if (target != null) chainTargets.Add(target);

            var otherEnemies = battleManager.allCombatants
              .Where(c => c.isPlayer != user.isPlayer && c.isAlive && c != target && !c.isVirtualTracker)
              .OrderBy(c => Vector3.Distance(target.transform.position, c.transform.position))
              .ToList();

            chainTargets.AddRange(otherEnemies);

            if (chainTargets.Count == 0) { battleManager.EndTurn(user); yield break; }

            // 2. Animation
            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(0.4f);

            // 3. Khởi tạo LineRenderer
            Flyweight_TB effectInstance = null;
            Vector3 startPos = user.SkillSpawnPoint != null ? user.SkillSpawnPoint.position : user.transform.position;

            if (skill.lazerSettings != null)
            {
                effectInstance = FlyweightFactory_TB.Spawn(skill.lazerSettings);
                if (effectInstance != null)
                {
                    effectInstance.Initialize(startPos, Quaternion.identity);
                    lineRenderer = effectInstance.GetComponent<LineRenderer>();

                    if (lineRenderer != null)
                    {
                        // QUAN TRỌNG: Tắt các script tự động điều khiển LineRenderer trên Prefab nếu có
                        var autoScripts = effectInstance.GetComponents<MonoBehaviour>();
                        foreach (var s in autoScripts)
                        {
                            if (s != effectInstance && s.GetType().Name != "LineRenderer") s.enabled = false;
                        }

                        lineRenderer.enabled = true;
                        lineRenderer.useWorldSpace = true;
                        lineRenderer.positionCount = chainTargets.Count + 1;

                        // Đặt tất cả các điểm về vị trí khởi đầu
                        for (int n = 0; n < lineRenderer.positionCount; n++)
                        {
                            lineRenderer.SetPosition(n, startPos);
                        }
                    }
                }
            }

            // 4. Bắt đầu nhảy điện
            int totalDamage = DamageCalculator.GetFinalDamage(user, chainTargets[0], skill, battleManager);
            int damagePerHit = totalDamage / Mathf.Max(1, skill.numberOfHits);

            for (int i = 0; i < chainTargets.Count; i++)
            {
                Character currentTarget = chainTargets[i];
                Vector3 targetPoint = currentTarget.transform.position + Vector3.up * 1.1f;

                if (lineRenderer != null)
                {
                    // Cập nhật điểm hiện tại trong chuỗi
                    lineRenderer.SetPosition(i + 1, targetPoint);

                    // Duy trì các điểm còn lại ở vị trí hiện tại để không bị kéo về 0,0,0
                    for (int j = i + 1; j < lineRenderer.positionCount; j++)
                    {
                        lineRenderer.SetPosition(j, targetPoint);
                    }
                }

                currentTarget.TakeDamage(user, damagePerHit, skill.elementType);
                SpawnImpactEffect(targetPoint, skill);

                if (i == 0) ApplyStatusEffectsAndStacks(user, currentTarget, skill);

                // Ép thời gian chờ tối thiểu là 0.2s để mắt người nhìn thấy
                float waitTime = Mathf.Max(0.2f, skill.delayBetweenHits);
                yield return new WaitForSeconds(waitTime);
            }

            // 5. Thu hồi tia điện lần lượt thay vì tắt toàn bộ cùng lúc
            if (lineRenderer != null)
            {
                // Lưu lại các vị trí hiện tại của tia điện để tính toán thu hồi
                Vector3[] finalPositions = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(finalPositions);

                // Thời gian thu hồi mỗi đoạn (thường nhanh hơn lúc phóng đi một chút cho mượt)
                float retractSpeed = Mathf.Max(0.1f, skill.delayBetweenHits * 0.7f);

                for (int i = 0; i < chainTargets.Count; i++)
                {
                    // Di chuyển các điểm gốc phía trước tới vị trí của điểm tiếp theo
                    // Cách này làm sợi dây ngắn lại từ phía Player
                    for (int k = 0; k <= i; k++)
                    {
                        lineRenderer.SetPosition(k, finalPositions[i + 1]);
                    }

                    // Nếu bạn muốn sợi dây thực sự biến mất (giảm số điểm) thì dùng:
                    // lineRenderer.positionCount--; // Nhưng cách dịch chuyển vị trí ở trên trông sẽ mượt hơn

                    yield return new WaitForSeconds(retractSpeed);
                }
            }

            // Cuối cùng mới tắt object
            if (effectInstance != null) effectInstance.gameObject.SetActive(false);

            user.animator.Play("Idle");
            yield return new WaitForSeconds(0.2f);
            battleManager.EndTurn(user);
        }
    }
}
