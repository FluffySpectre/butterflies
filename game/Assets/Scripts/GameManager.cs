using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ButterflyController playerController;
    public Transform hand;
    public Vector3 handOnDomePosition;
    public Vector3 handInTheSkyPosition;
    public float handFallSpeed = 5f;

    public event Action OnHandUpdated;
    
    public float CurrentHandPosition { get; private set; }

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
        // UpdateHand();
    }
 
    private void UpdateHand()
    {
        CurrentHandPosition += Time.deltaTime / handFallSpeed;
        hand.position = Vector3.Lerp(handInTheSkyPosition, handOnDomePosition, CurrentHandPosition);
        OnHandUpdated?.Invoke();
    }

    public void OnApproachFlower(GameObject flower, GameObject interactor) 
    {
        StartCoroutine(ApproachFlowerRoutine(flower, interactor));
    }

    private IEnumerator ApproachFlowerRoutine(GameObject flower, GameObject interactor)
    {
        var butterflyController = interactor.GetComponentInParent<ButterflyController>();
        butterflyController.controllable = false;

        var flowerCam = flower.transform.parent.Find("Virtual Camera").gameObject;
        if (interactor.layer == LayerMask.NameToLayer("Player"))
        {
            flowerCam.SetActive(true);
        }

        yield return new WaitForSeconds(5);

        if (interactor.layer == LayerMask.NameToLayer("Player"))
        {
            flowerCam.SetActive(false);
        }
        
        yield return new WaitForSeconds(2);

        butterflyController.controllable = true;
    }
}
