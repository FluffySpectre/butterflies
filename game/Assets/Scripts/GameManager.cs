using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ButterflyController playerController;
    public GameObject hand;
    public event Action OnHandUpdated;
    public float timeBeforeFirstHandEvent = 20f;

    private bool handEventTriggered = false;
    private ButterflyController[] butterflies;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.OnApproachFlower += OnApproachFlower;
    }

    void OnDestroy()
    {
        GameEvents.Instance.OnApproachFlower -= OnApproachFlower;
    }

    // Update is called once per frame
    void Update()
    {
        if (butterflies == null || butterflies.Length < 3)
        {
            FindAllAIButterflies();
        }

        if (!handEventTriggered && Time.time > timeBeforeFirstHandEvent)
        {
            handEventTriggered = true;

            ActivateHand();
            SetButterflyAlertState(true);
        }
    }
 
    private void ActivateHand()
    {
        hand.SetActive(true);
    }

    private void FindAllAIButterflies()
    {
        butterflies = FindObjectsOfType<ButterflyController>();
        Debug.Log("Found butterflies: " + butterflies.Length);
    }

    private void SetButterflyAlertState(bool alerted)
    {
        for (int i = 0; i < butterflies.Length; i++)
        {
            var b = butterflies[i];

            if (b.TryGetComponent(out AIPlayerInput aiPlayerInput))
            {
                b.speed = 100f;
                b.turnSpeed = 100f;
                b.pitchSpeed = 90f;
                b.maxWingFlapSpeed = 20f;

                aiPlayerInput.minTimeUntilPickANewTarget = 1f;
                aiPlayerInput.maxTimeUntilPickANewTarget = 5f;
            }
        }
    }

    public void OnApproachFlower(GameObject flower, GameObject interactor) 
    {
        StartCoroutine(ApproachFlowerRoutine(flower, interactor));
    }

    private IEnumerator ApproachFlowerRoutine(GameObject flower, GameObject interactor)
    {
        var butterflyController = interactor.GetComponentInParent<ButterflyController>();
        butterflyController.controllable = false;

        // var flowerCam = flower.transform.parent.Find("Virtual Camera").gameObject;
        // if (interactor.layer == LayerMask.NameToLayer("Player"))
        // {
        //     flowerCam.SetActive(true);
        // }

        // Move closer and look at the flower
        var startRotation = interactor.transform.rotation;
        var targetRotation = Quaternion.LookRotation(flower.transform.forward, Vector3.up);
        var startPosition = interactor.transform.position;
        var targetPosition = flower.transform.position;

        float lerpSpeed = 1.0f;
        float lerpProgress = 0f;

        while (lerpProgress < 1.0f)
        {
            lerpProgress += Time.deltaTime * lerpSpeed;
            interactor.transform.SetPositionAndRotation(
                Vector3.Lerp(startPosition, targetPosition, lerpProgress),
                Quaternion.Slerp(startRotation, targetRotation, lerpProgress)
            );
            yield return null;
        }

        // TODO: do nectar stuff

        yield return new WaitForSeconds(3);

        // Turn away from the flower
        startRotation = interactor.transform.rotation;
        targetRotation = Quaternion.LookRotation(-flower.transform.forward, Vector3.up);
        lerpSpeed = 1.0f;
        lerpProgress = 0f;
        while (lerpProgress < 1.0f)
        {
            lerpProgress += Time.deltaTime * lerpSpeed;
            interactor.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, lerpProgress);
            yield return null;
        }

        // if (interactor.layer == LayerMask.NameToLayer("Player"))
        // {
        //     flowerCam.SetActive(false);
        // }
        
        // yield return new WaitForSeconds(2);

        butterflyController.controllable = true;
    }
}
