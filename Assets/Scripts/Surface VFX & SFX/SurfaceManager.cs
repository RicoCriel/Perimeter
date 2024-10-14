using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class SurfaceManager : MonoBehaviour
{
    [Header("Surface Properties")]
    [SerializeField]
    private List<SurfaceType> Surfaces = new List<SurfaceType>();
    [SerializeField]
    private int _defaultPoolSize = 10;
    [SerializeField]
    private Surface _defaultSurface;
    [SerializeField] private Transform _pooledObjectsGroup;

    [Header("Score Element Reference")]
    [SerializeField] private TextMeshProUGUI _scoreText;

    private Dictionary<GameObject, Renderer> _rendererCache = new Dictionary<GameObject, Renderer>();
    private Dictionary<GameObject, ObjectPool<GameObject>> objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    private Dictionary<AudioSource, ObjectPool<GameObject>> audioPools = new Dictionary<AudioSource, ObjectPool<GameObject>>();

    private static SurfaceManager _instance;
    public static SurfaceManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void HandleImpact(GameObject HitObject, Vector3 HitPoint, Vector3 HitNormal, ImpactType Impact, int TriangleIndex)
    {
        if (HitObject.TryGetComponent<Terrain>(out Terrain terrain))
        {
            List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, HitPoint);
            foreach (TextureAlpha activeTexture in activeTextures)
            {
                SurfaceType surfaceType = Surfaces.Find(surface => surface.AlbedoTexture == activeTexture.Texture);
                if (surfaceType != null)
                {
                    foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == Impact)
                        {
                            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, /*activeTexture.Alpha,*/ 1f);
                        }
                    }
                }
                else
                {
                    PlayDefaultEffects(HitPoint, HitNormal, Impact);
                }
            }
        }
        else
        {
            Renderer renderer = FindRenderer(HitObject);

            if (renderer != null)
            {
                Texture activeTexture = GetActiveTextureFromRenderer(renderer, TriangleIndex);
                SurfaceType surfaceType = Surfaces.Find(surface => surface.AlbedoTexture == activeTexture);

                if (surfaceType != null)
                {
                    foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == Impact)
                        {
                            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1f);
                            if (surfaceType.Type == Type.Enemy)
                            {
                                ScoreManager.Instance.IncreaseScore(10, _scoreText);
                            }
                        }
                    }
                }
                else
                {
                    PlayDefaultEffects(HitPoint, HitNormal, Impact);
                }
            }
            else
            {
                Debug.LogError($"{HitObject.name} has no Renderer! Using default impact effect.");
                PlayDefaultEffects(HitPoint, HitNormal, Impact);
            }
        }
    }

    private Renderer FindRenderer(GameObject HitObject)
    {
        if (_rendererCache.TryGetValue(HitObject, out Renderer cachedRenderer))
        {
            return cachedRenderer;
        }

        if (HitObject.TryGetComponent<Renderer>(out var renderer))
        {
            _rendererCache[HitObject] = renderer;
            return renderer;
        }

        for (int i = 0; i < HitObject.transform.childCount; i++)
        {
            Transform child = HitObject.transform.GetChild(i);
            if (child.TryGetComponent<Renderer>(out renderer))
            {
                _rendererCache[HitObject] = renderer;
                return renderer;
            }
        }

        return null;
    }

    private void PlayDefaultEffects(Vector3 HitPoint, Vector3 HitNormal, ImpactType Impact)
    {
        foreach (Surface.SurfaceImpactTypeEffect typeEffect in _defaultSurface.ImpactTypeEffects)
        {
            if (typeEffect.ImpactType == Impact)
            {
                PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
            }
        }
    }

    private List<TextureAlpha> GetActiveTexturesFromTerrain(Terrain terrain, Vector3 HitPoint)
    {
        Vector3 terrainPosition = HitPoint - terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(
            terrainPosition.x / terrain.terrainData.size.x,
            0,
            terrainPosition.z / terrain.terrainData.size.z
        );

        int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

        List<TextureAlpha> activeTextures = new List<TextureAlpha>();
        for (int i = 0; i < alphaMap.Length; i++)
        {
            if (alphaMap[0, 0, i] > 0)
            {
                activeTextures.Add(new TextureAlpha()
                {
                    Texture = terrain.terrainData.terrainLayers[i].diffuseTexture,
                    Alpha = alphaMap[0, 0, i]
                });
            }
        }

        return activeTextures;
    }

    private Texture GetActiveTextureFromRenderer(Renderer renderer, int TriangleIndex)
    {
        Mesh mesh = null;

        if (renderer is MeshRenderer && renderer.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            mesh = meshFilter.mesh;
        }
        else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
        {
            mesh = skinnedMeshRenderer.sharedMesh;
        }

        if (mesh == null)
        {
            Debug.LogError($"{renderer.name} has no mesh! Using default impact effect.");
            return null;
        }

        if (mesh.subMeshCount > 1)
        {
            int[] hitTriangleIndices = new int[]
            {
                mesh.triangles[TriangleIndex * 3],
                mesh.triangles[TriangleIndex * 3 + 1],
                mesh.triangles[TriangleIndex * 3 + 2]
            };

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] submeshTriangles = mesh.GetTriangles(i);
                for (int j = 0; j < submeshTriangles.Length; j += 3)
                {
                    if (submeshTriangles[j] == hitTriangleIndices[0]
                        && submeshTriangles[j + 1] == hitTriangleIndices[1]
                        && submeshTriangles[j + 2] == hitTriangleIndices[2])
                    {
                        return renderer.sharedMaterials[i].mainTexture;
                    }
                }
            }
        }
        else
        {
            return renderer.sharedMaterial.mainTexture;
        }

        return null;
    }

    private void PlayEffects(Vector3 hitPoint, Vector3 hitNormal, SurfaceEffect surfaceEffect, float soundOffset)
    {
        foreach (SpawnObjectEffect spawnObjectEffect in surfaceEffect.SpawnObjectEffects)
        {
            if (spawnObjectEffect.Probability > Random.value)
            {
                GameObject prefab = spawnObjectEffect.Prefab;
                ObjectPool<GameObject> pool;

                // Ensure pool exists for the prefab, if not, create one
                if (!objectPools.TryGetValue(prefab, out pool))
                {
                    pool = CreateObjectPool(prefab);
                    objectPools[prefab] = pool;  // Store the pool in the dictionary
                }

                // Get object from pool
                GameObject instance = pool.Get();
                instance.transform.position = hitPoint + hitNormal * 0.001f;
                instance.transform.forward = hitNormal;

                // Set the parent pool in the PoolableObject
                if (instance.TryGetComponent<PoolableObject>(out var poolableObject))
                {
                    poolableObject.SetParentPool(pool);
                }

                if (spawnObjectEffect.RandomizeRotation)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.x),
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.y),
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.z)
                    );
                    instance.transform.rotation = Quaternion.Euler(instance.transform.rotation.eulerAngles + offset);
                }

                StartCoroutine(ReturnToPoolAfterDuration(instance, spawnObjectEffect.Lifetime, pool));
            }
        }

        foreach (PlayAudioEffect playAudioEffect in surfaceEffect.PlayAudioEffects)
        {
            AudioClip clip = playAudioEffect.AudioClips[Random.Range(0, playAudioEffect.AudioClips.Count)];

            // Get audio source pool
            ObjectPool<GameObject> audioPool;
            if (!audioPools.TryGetValue(playAudioEffect.AudioSourcePrefab, out audioPool))
            {
                audioPool = CreateObjectPool(playAudioEffect.AudioSourcePrefab.gameObject);
                audioPools[playAudioEffect.AudioSourcePrefab] = audioPool;
            }

            // Get audio instance from pool
            GameObject audioInstance = audioPool.Get();
            AudioSource audioSource = audioInstance.GetComponent<AudioSource>();

            audioInstance.transform.position = hitPoint;

            // Create variety with rapid hits
            float randomPitch = Random.Range(0.7f, 1.1f);  
            audioSource.pitch = randomPitch;
            float randomVolume = Random.Range(playAudioEffect.VolumeRange.x, playAudioEffect.VolumeRange.y);
            audioSource.PlayOneShot(clip, soundOffset * randomVolume);
        }
    }

    private ObjectPool<GameObject> CreateObjectPool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, _pooledObjectsGroup),
            actionOnGet: obj => {
                obj.SetActive(true);
                obj.transform.SetParent(_pooledObjectsGroup);  // Ensure the parent is set on retrieval
            },
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            defaultCapacity: _defaultPoolSize,
            maxSize: _defaultPoolSize  // Ensure max size is respected
        );
    }

    private IEnumerator ReturnToPoolAfterDuration(GameObject instance, float duration, ObjectPool<GameObject> pool)
    {
        yield return new WaitForSeconds(duration);

        PoolableObject poolableObject = instance.GetComponent<PoolableObject>();
        if (poolableObject != null)
        {
            poolableObject.ReturnToPool();
        }
        else
        {
            pool.Release(instance);
        }
    }

    private IEnumerator DisableAudioSource(GameObject audioInstance, float duration, ObjectPool<GameObject> audioPool)
    {
        yield return new WaitForSeconds(duration);
        audioPool.Release(audioInstance);
    }

    [System.Serializable]
    public struct TextureAlpha
    {
        public Texture Texture;
        public float Alpha;
    }
}
