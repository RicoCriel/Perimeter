using UnityEngine;

public class PlayerView : MonoBehaviour
{
    //This class is only responsible for updating the visuals of the player
    public Transform MapCenter;
    public float InteractRange;
    public float MapRadius;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void UpdateAnimation(Vector3 input)
    {
        _animator.SetFloat("Speed", input.magnitude);
    }

    public void TriggerAnimation(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }

    public void ResetTriggerAnimation(string triggerName)
    {
        _animator.ResetTrigger(triggerName);
    }

    public void LookAtDirection(Vector3 direction, CharacterController characterController, float rotationSpeed, PlayerModel model)
    {
        if (direction.sqrMagnitude > 0.01f || model.MoveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction == Vector3.zero ?
                                                                new Vector3(model.MoveInput.x, 0, model.MoveInput.y) :
                                                                direction);
            characterController.transform.rotation = Quaternion.RotateTowards(
                characterController.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void KeepWithinUnitCircle(Vector3 position, float radius)
    {
        Vector3 offset = transform.position - position;
        if (offset.magnitude > radius)
        {
            //Show leaving objective area + kill player if staying too long here
            //transform.position = position + offset.normalized * radius;
        }
    }
}
