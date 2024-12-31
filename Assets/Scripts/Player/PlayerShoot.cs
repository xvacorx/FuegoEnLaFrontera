using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerShoot : MonoBehaviour
{
    public string category;
    public string pool;
    [SerializeField] Transform firePoint;

    private InputSystem_Actions playerInputActions;
    private InputAction fireAction;

    void Awake()
    {
        playerInputActions = new InputSystem_Actions();
        fireAction = playerInputActions.Player.Attack;
    }

    void OnEnable()
    {
        fireAction.Enable();
    }

    void OnDisable()
    {
        fireAction.Disable();
    }

    void Update()
    {
        if (fireAction.triggered)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject projectile = PoolManager.Instance.RequestObject(category, pool);
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;
        projectile.gameObject.SetActive(true);
    }
}
