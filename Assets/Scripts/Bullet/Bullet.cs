using System.Collections;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float speed;
    [SerializeField] private float lifetime = 2f;

    private void OnEnable()
    {
        StartCoroutine(DeactivateAfterTime(lifetime));
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private IEnumerator DeactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
