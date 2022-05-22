using UnityEngine;

public class Disabler : MonoBehaviour
{
    [SerializeField] bool onlyDisableForDestroyer;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyer") && onlyDisableForDestroyer)
        {
            if (gameObject.layer == 8) transform.parent.gameObject.SetActive(false);
            else gameObject.SetActive(false);
        }
        else if (other.CompareTag("Player") || other.CompareTag("Destroyer")) gameObject.SetActive(false);
    }
}