using UnityEngine;

public class Disabler : MonoBehaviour
{
    [SerializeField] bool onlyDisableForDestroyer;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyer") && onlyDisableForDestroyer) gameObject.SetActive(false);
        else if (other.CompareTag("Player") || other.CompareTag("Destroyer")) gameObject.SetActive(false);
    }
}