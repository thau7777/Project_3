using UnityEngine;

namespace MyRule.UI
{
    public class GraphicView : MonoBehaviour
    {
        public void SetFullScreen(int index)
        {
            if (index == 0) 
            {
                GraphicManager.Instance.SetFullscreen(true);
            }
            else if (index == 1)
            {
                GraphicManager.Instance.SetFullscreen(false);
            }
        }

        public void SetResolution(int index)
        {
            GraphicManager.Instance.SetResolution(index);
        }
    }
}