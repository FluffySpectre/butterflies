using UnityEngine;

public class GroundParticles : MonoBehaviour
{
    public ParticleSystem particles;
    public float startHeight = 20f;
    public float forwardAheadOffset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        particles.Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, startHeight))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                if (!particles.isPlaying)
                    particles.Play();
                particles.transform.SetPositionAndRotation(hit.point + transform.forward * forwardAheadOffset, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                if (particles.isPlaying)
                    particles.Stop();
            }
        }
        else
        {
            if (particles.isPlaying)
                particles.Stop();
        }
    }
}
