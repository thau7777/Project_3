using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

namespace Turnbase
{
    // Định nghĩa các loại Cutscene trong trận đấu
    public enum BattleCutsceneType
    {
        Start,
        IntroWave,
        BossEntrance,
        VictoryEnding
    }

    [Serializable]
    public struct CutsceneData
    {
        public BattleCutsceneType type;
        public PlayableAsset timeline;
    }

    public class BattleCutsceneManager : MonoBehaviour
    {
        public static BattleCutsceneManager Instance;

        [SerializeField] private PlayableDirector director;
        [SerializeField] private List<CutsceneData> cutsceneList;

        private BattleManager battleManager;

        private void Awake()
        {
            Instance = this;
            battleManager = GetComponent<BattleManager>();
            if (director == null) director = GetComponent<PlayableDirector>();
        }

        public async UniTask PlayCutscene(BattleCutsceneType type)
        {
            // Tìm timeline tương ứng trong danh sách
            var data = cutsceneList.Find(x => x.type == type);

            if (data.timeline == null)
            {
                Debug.LogWarning($"Cutscene {type} chưa được gán Timeline!");
                return;
            }

            // 1. Khóa trận đấu
            SetBattleState(true);

            // 2. Chạy Timeline
            director.playableAsset = data.timeline;
            director.Play();

            Debug.Log($"<color=cyan>[CUTSCENE]</color> Bắt đầu: {type}");

            // 3. Đợi cho đến khi kết thúc (hoặc gần kết thúc)
            await UniTask.WaitUntil(() => director.state != PlayState.Playing);

            // 4. Mở khóa trận đấu
            SetBattleState(false);

            Debug.Log($"<color=cyan>[CUTSCENE]</color> Kết thúc: {type}");
        }

        private void SetBattleState(bool isPaused)
        {
            if (battleManager == null) return;

            battleManager.isProcessingTurn = isPaused;
            if (battleManager.turnHandler != null)
            {
                battleManager.turnHandler.isProcessingTurn = isPaused;
            }
        }
    }
}