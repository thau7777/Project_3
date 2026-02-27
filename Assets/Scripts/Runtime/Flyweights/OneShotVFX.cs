using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.VFX;

public class OneShotVFX : Flyweight
{
    new OneShotVFXSettings settings => (OneShotVFXSettings)base.settings;
    public float Damage { get; set; }
    private Coroutine _despawnCoroutine;
    private Coroutine _decalSpawnCoroutine;

    private void OnEnable()
    {
        //Damage = settings.Damage;
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
            _decalSpawnCoroutine = null;
        }
        if (_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
        
    }

    private void OnDisable()
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
            _despawnCoroutine = null;
        }

        if (_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
    }
    private void CheckAndSetProperty<T>(string propertyName, T value)
    {
        var vfxGraph = GetComponent<VisualEffect>();

        if (vfxGraph == null)
            return;

        // Check and set the property based on its type
        if (typeof(T) == typeof(float))
        {
            if (vfxGraph.HasFloat(propertyName))
                vfxGraph.SetFloat(propertyName, (float)(object)value);
        }
        else if (typeof(T) == typeof(int))
        {
            if (vfxGraph.HasInt(propertyName))
                vfxGraph.SetInt(propertyName, (int)(object)value);
        }
        else if (typeof(T) == typeof(bool))
        {
            if (vfxGraph.HasBool(propertyName))
                vfxGraph.SetBool(propertyName, (bool)(object)value);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            if (vfxGraph.HasVector3(propertyName))
                vfxGraph.SetVector3(propertyName, (Vector3)(object)value);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            if (vfxGraph.HasVector2(propertyName))
                vfxGraph.SetVector2(propertyName, (Vector2)(object)value);
        }
        else if (typeof(T) == typeof(Mesh))
        {
            if (vfxGraph.HasMesh(propertyName))
                vfxGraph.SetMesh(propertyName, (Mesh)(object)value);
        }
        else
        {
            Debug.LogWarning($"Unsupported property type: {typeof(T)} for property: {propertyName}");
        }
    }
    public void InitializeVFX(float size, float lifeTime)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(size, size, size);
            
        if (settings.useAdvanceSettings)
        {
            //CheckAndSetProperty(settings.sizeName, size);
            CheckAndSetProperty(settings.durationName, lifeTime);
            GetComponent<VisualEffect>().SendEvent(settings.playEventName);
        }
        else
        {
            //transform.localScale = new Vector3(size, size, size);
        }
        if (settings.decalSettings)
        {
            if (settings.decalDelayTime > lifeTime)
                Debug.LogWarning("decal delay time is bigger than the effect lifeTime");
            else
            {
                if (settings.decalDelayTime <= 0)
                    SpawnDecal(settings.decalSize, settings.decalDuration);
                else
                    _decalSpawnCoroutine = StartCoroutine(DecalSpawnCoroutine(settings.decalSize, settings.decalDelayTime, settings.decalDuration));
            }
        }

        if (settings.HasHitBox && !settings.UseParticleCollision) 
            GetComponent<HitBoxHandler>().StartHitBoxCoroutine(lifeTime);
        //if(settings.CanDealDamage)
        //    GetComponent<DamageDealer>().Damage = Damage;
        _despawnCoroutine = StartCoroutine(LifetimeRoutine(lifeTime));
    }
    private IEnumerator LifetimeRoutine(float lifeTime)
    {
        yield return Helpers.GetWaitForSeconds(lifeTime);
        FlyweightFactory.ReturnToPool(this);
    }
    private void SpawnDecal(float size, float decalDuration)
    {
        DecalProjectorController decal = FlyweightFactory.Spawn(settings.decalSettings).GetComponent<DecalProjectorController>();
        decal.transform.position = transform.position.Add(y:3);
        decal.SetMaterial(settings.decalMaterial);
        decal.SetSize(size, size);
        decal.FadeOut(decalDuration);
    }
    private IEnumerator DecalSpawnCoroutine(float size, float decalDelayTime, float decalDuration)
    {
        yield return Helpers.GetWaitForSeconds(decalDelayTime);
        SpawnDecal(size, decalDuration);
    }
}