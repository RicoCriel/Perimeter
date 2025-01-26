using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRigidbodyPush : MonoBehaviour
{
    public LayerMask PushLayers;
    [SerializeField] private bool _canPush;
    [Range(0.5f, 5f)]
    [SerializeField] private float _strength;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_canPush) PushRigidBodies(hit);
    }

    private void PushRigidBodies(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if ((body == null) || body.isKinematic) return;

        var bodyLayerMask = 1 << body.gameObject.layer;
        if((bodyLayerMask & PushLayers.value) == 0) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
        body.AddForce(pushDir * _strength, ForceMode.Impulse);
    }
}
