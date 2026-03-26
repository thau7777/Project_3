using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PixPlays.ElementalVFX
{
    public class ProjectileVfx : BaseVfx
    {
        [SerializeField, Range(0, 1)] private float _stopChasingThreshold = 1f;
        [SerializeField] ParticleSystem _CastEffect;
        [SerializeField] ParticleSystem _HitEffect;
        [SerializeField] ParticleSystem _ProjectileEffect;
        [SerializeField] float _flightDuration = 1;
        [SerializeField] AnimationCurve _FlyCurve;
        [SerializeField] bool _RandomizeFlyCurveDirection;
        [SerializeField, ShowIf("_RandomizeFlyCurveDirection", true)] Vector2 _FlyCurveDirection;
        [SerializeField, ShowIf("_RandomizeFlyCurveDirection"), MinMaxSlider(-1,1)] Vector2 _randomizedMinMaxX;
        [SerializeField, ShowIf("_RandomizeFlyCurveDirection"), MinMaxSlider(-1, 1)] Vector2 _randomizedMinMaxY;
        [SerializeField] float _FlyCurveStrength;
        [SerializeField] float _ProjectileFlyDelay;
        [SerializeField] float _ProjectileDeactivateDelay;

        [SerializeField] private LayerMask _ExplosionLayers;
        [SerializeField] private float _CollisionRadius = 0.2f;
        [SerializeField] private float _MaxFlightTime = 5f;

        public override void Play(VfxData data)
        {
            base.Play(data);
            StartCoroutine(Coroutine_Projectile());
        }

        IEnumerator Coroutine_Projectile()
        {
            _CastEffect.gameObject.SetActive(true);
            _CastEffect.transform.position = transform.position;
            _CastEffect.transform.forward = (_data.Target - _data.Source);
            _CastEffect.Play();

            yield return new WaitForSeconds(_ProjectileFlyDelay);

            _ProjectileEffect.gameObject.SetActive(true);
            _ProjectileEffect.transform.position = _CastEffect.transform.position;
            _ProjectileEffect.Play();

            //_FlyCurveDirection = _FlyCurveDirection.normalized;
            if (_RandomizeFlyCurveDirection)
            {
                _FlyCurveDirection = new Vector2(Random.Range(_randomizedMinMaxX.x, _randomizedMinMaxX.y), Random.Range(_randomizedMinMaxY.x, _randomizedMinMaxY.y));
            }

            float lerp = 0;
            float flightTime = 0f;
            bool stoppedChasing = false;
            bool hitSomething = false;
            Vector3 startPos = _ProjectileEffect.transform.position;

            while (lerp < 1)
            {
                flightTime += Time.deltaTime;

                // --- Check flight time ---
                if (flightTime >= _MaxFlightTime)
                    break;

                Vector3 prevPos = _ProjectileEffect.transform.position;

                Vector3 pos = Vector3.Lerp(startPos, _data.Target, lerp);
                pos += (Vector3)_FlyCurveDirection * _FlyCurve.Evaluate(lerp) * _FlyCurveStrength;

                if (lerp > 0)
                    _ProjectileEffect.transform.forward = (pos - prevPos);

                _ProjectileEffect.transform.position = pos;
                lerp += Time.deltaTime / _flightDuration;

                // --- Check hit layer ---
                Vector3 travelDelta = pos - prevPos;
                if (travelDelta.magnitude > 0 && Physics.SphereCast(prevPos, _CollisionRadius, travelDelta.normalized,
                    out RaycastHit hit, travelDelta.magnitude, _ExplosionLayers))
                {
                    _ProjectileEffect.transform.position = hit.point;
                    hitSomething = true;
                    break;
                }

                // --- Threshold check ---
                if (!stoppedChasing && _stopChasingThreshold < 1f && lerp >= _stopChasingThreshold)
                {
                    stoppedChasing = true;
                    Vector3 straightDirection = _ProjectileEffect.transform.forward;
                    float straightTimer = 0f;

                    while (straightTimer < 1f)
                    {
                        flightTime += Time.deltaTime;
                        prevPos = _ProjectileEffect.transform.position;
                        _ProjectileEffect.transform.position += straightDirection * Time.deltaTime * (_data.Target - _data.Source).magnitude / _flightDuration;

                        // Check layers and flight time during straight travel too
                        Vector3 straightDelta = _ProjectileEffect.transform.position - prevPos;
                        if (straightDelta.magnitude > 0 && Physics.SphereCast(prevPos, _CollisionRadius, straightDelta.normalized,
                            out RaycastHit straightHit, straightDelta.magnitude, _ExplosionLayers))
                        {
                            _ProjectileEffect.transform.position = straightHit.point;
                            hitSomething = true;
                            break;
                        }

                        if (flightTime >= _MaxFlightTime)
                            break;

                        straightTimer += Time.deltaTime;
                        yield return null;
                    }

                    break;
                }

                yield return null;
            }

            // Explode wherever the projectile ended up
            Vector3 explodePos = _ProjectileEffect.transform.position;
            if (!stoppedChasing && !hitSomething)
                explodePos = _data.Target;

            _HitEffect.transform.forward = hitSomething
                ? -_ProjectileEffect.transform.forward
                : (_ProjectileEffect.transform.position - _data.Target);

            _ProjectileEffect.transform.position = explodePos;
            _ProjectileEffect.Stop();

            _HitEffect.transform.position = explodePos;
            _HitEffect.gameObject.SetActive(true);
            _HitEffect.Play();

            if (_HitEffect.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
                hitBoxHandler.StartHitBoxCoroutine(hitBoxLifeTime);


            yield return new WaitForSeconds(_ProjectileDeactivateDelay);
            _ProjectileEffect.gameObject.SetActive(false);
        }

        public override void Stop()
        {
            base.Stop();
            if (gameObject != null)
            {
                _HitEffect.Stop();
                _ProjectileEffect.Stop();
                _CastEffect.Stop();
            }
        }
    }
}