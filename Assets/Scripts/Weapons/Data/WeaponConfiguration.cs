using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "WeaponConfiguration", menuName = "Weapons/WeaponConfiguration", order = 0)]
public class WeaponConfiguration : ScriptableObject
{
    //public ImpactType ImpactType;
    //public GameObject WeaponModelPrefab;
    public ShootConfiguration ShootConfig;
    public TrailConfiguration TrailConfig;

    private MonoBehaviour _activeMonoBehaviour;
    private float _lastShootTime;
    private ObjectPool<TrailRenderer> _trailPool;

    public void ActivateTrail(MonoBehaviour activeMonoBehaviour)
    {
        this._activeMonoBehaviour = activeMonoBehaviour;
        _lastShootTime = 0;
        _trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

    }

    public void Shoot(ParticleSystem shootSystem)
    {
        if (Time.time > ShootConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;

            // Make sure the particle system is in the correct position
            shootSystem.Play();

            // Generate random spread direction
            Vector3 spreadDirection = new Vector3(
                Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
            );

            // Get the current forward direction from the weapon (shooting direction)
            Vector3 shootDirection = shootSystem.transform.forward + spreadDirection;
            shootDirection.Normalize();

            // Get the current world position of the particle system as the start point
            Vector3 startPosition = shootSystem.transform.position;

            // Check if we hit something
            if (Physics.Raycast(startPosition, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
            {
                // Start the bullet trail to the hit point
                _activeMonoBehaviour.StartCoroutine(PlayTrail(startPosition, hit.point, hit));
            }
            else
            {
                // No hit, so create a trail going forward into the distance
                _activeMonoBehaviour.StartCoroutine(PlayTrail(
                    startPosition,
                    startPosition + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit() // Empty hit info for a miss
                ));
            }
        }
    }


    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = _trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null; // avoid previous trailrenderer artifacts

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        bool hasReleased = false;
        while (remainingDistance > 0)
        { 
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, 
                Mathf.Clamp01(1 - remainingDistance/ distance));

            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;

            instance.transform.position = endPoint;

            if(hit.collider != null)
            {
                //Handle impact
                Debug.Log(hit.collider.name);
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);

            if (!hasReleased)
            {
                _trailPool.Release(instance);
                hasReleased = true; // Set the flag to prevent further releases
            }
        }
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }


}
