using UnityEngine;

public static class Helpers
{
    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        return WaitFor.Seconds(seconds);
    }
}
