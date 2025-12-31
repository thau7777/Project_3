namespace Turnbase
{
    public class CameraShakeEvent
    {
        public float duration;
        public float magnitude;

        public CameraShakeEvent(float duration, float magnitude)
        {
            this.duration = duration;
            this.magnitude = magnitude;
        }
    }
}