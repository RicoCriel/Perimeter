using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Singleton")]
    public static PlayerController instance;
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [Header("Map Bounds")]
    [SerializeField] private Transform mapCenter;
    [SerializeField] private float mapRadius;

    private Player player;
    private CharacterController characterController;
    private Animator animator;

    private void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = new Player(moveSpeed, characterController);
    }

    private void Update()
    {
        RotatePlayer();
        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;
        player.Move(movement, Time.deltaTime);
        player.UpdateAnimation(animator,movement);
        player.KeepWithinUnitCircle(mapCenter.position, mapRadius);
    }

    private void RotatePlayer()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        player.RotateTowards(mouseWorldPosition);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float distanceToPlane;

        if (playerPlane.Raycast(ray, out distanceToPlane))
        {
            return ray.GetPoint(distanceToPlane);
        }

        return transform.position;
    }
}
