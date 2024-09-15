using UnityEngine;

public class HumanPlayerInput : MonoBehaviour
{
    private ButterflyController butterflyController;

    private void Awake()
    {
        butterflyController = GetComponent<ButterflyController>();
    }

    private void Update()
    {
        butterflyController.ApplyInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
