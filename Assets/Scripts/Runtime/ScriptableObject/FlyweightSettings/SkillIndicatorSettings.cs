using UnityEngine;

[CreateAssetMenu(fileName = " New SkillIndicator Settings", menuName = "Scriptable Objects/Flyweight/SkillIndicator Settings")]
public class SkillIndicatorSettings : FlyweightSettings
{
    public bool canLockIn;
    public LayerMask groundMask = ~0; // all layers by default
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        Flyweight flyweight = null;
        switch (type)
        {
            case FlyweightType.IndicatorCircleAlly:
                {
                    flyweight = go.GetOrAdd<CircleIndicator>();
                    break;
                }
            case FlyweightType.IndicatorStraightAlly:
                {
                    flyweight = go.GetOrAdd<FollowedIndicator>(); 
                    break;
                }
        }

        flyweight.settings = this;

        return flyweight;
    }

    //public override void OnGet(Flyweight f)
    //{

    //}
}


