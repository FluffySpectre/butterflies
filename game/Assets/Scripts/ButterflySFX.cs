using UnityEngine;

public class ButterflySFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] glassHitClips;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "DomeCollider")
        {
            audioSource.clip = glassHitClips[Random.Range(0, glassHitClips.Length)];
            audioSource.Play();
        }
    }
}
