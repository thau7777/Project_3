using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public class TB_SpeedRun : MonoBehaviour
    {
        public float speedRunLevel = 2;
        public void SpeedRun()
        {
            Time.timeScale = (Time.timeScale == 1f) ? speedRunLevel : 1f;
        }
    }

}