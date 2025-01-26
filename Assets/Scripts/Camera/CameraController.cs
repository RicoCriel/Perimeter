using UnityEngine;
using CameraShake;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private LayerMask _obstructionMask;
    [SerializeField] private Material _transparentMaterial;

    private Dictionary<Renderer, Material> _originalMaterials = new Dictionary<Renderer, Material>();
    [SerializeField] private float _duration;

    private void LateUpdate()
    {
        FollowTarget();
        //HandleObstructions();
    }

    private void FollowTarget()
    {
        if (_target != null)
        {
            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    private void HandleObstructions()
    {
        if (_target == null) return;

        // Clear transparency from previously obstructing objects
        foreach (Renderer renderer in _originalMaterials.Keys)
        {
            SetObjectTransparency(renderer, false);
        }
        _originalMaterials.Clear();

        // Perform raycasting to find obstructions
        Vector3 directionToCamera = transform.position - _target.position;
        float distanceToCamera = directionToCamera.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(_target.position, directionToCamera.normalized, distanceToCamera, _obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (!_originalMaterials.ContainsKey(renderer)) // Only store material if not already stored
                {
                    _originalMaterials[renderer] = renderer.material;
                }
                SetObjectTransparency(renderer, true);
            }
        }
    }

    private void SetObjectTransparency(Renderer renderer, bool switchMaterial)
    {
        if (switchMaterial)
        {
            renderer.material = _transparentMaterial;
        }
        else
        {
            if (_originalMaterials.ContainsKey(renderer))
            {
                renderer.material = _originalMaterials[renderer];
            }
        }
    }

    public void ShakeCamera()
    {
        CameraShaker.Presets.ShortShake3D();
    }
}
