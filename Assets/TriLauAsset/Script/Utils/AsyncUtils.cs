using System.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public static class AsyncUtils
    {
        public static async Task WaitForSeconds(float seconds)
        {
            if (seconds < 0f)
                seconds = 0f;

            int milliseconds = Mathf.RoundToInt(seconds * 1000f);
            await Task.Delay(milliseconds);
        }
    }
}