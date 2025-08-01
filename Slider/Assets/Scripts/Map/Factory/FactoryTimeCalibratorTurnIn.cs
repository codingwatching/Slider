using UnityEngine;

public class FactoryTimeCalibratorTurnIn : MonoBehaviour
{
    public const string CALIBRATOR_SAVE_STRING = "factoryTimeCalibratorTurnedIn";

    [SerializeField] private Item timeCalibratorItem;
    [SerializeField] private GameObject timeCalibratorStatic;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform timeCalibratorFinalPosition;

    private void Start()
    {
        if (SaveSystem.Current.GetBool(CALIBRATOR_SAVE_STRING))
        {
            MoveCalibratorToCorrectPosition();
        }
    }

    public void TurnInCalibrator()
    {
        if (SaveSystem.Current.GetBool(CALIBRATOR_SAVE_STRING))
        {
            return;
        }
        SaveSystem.Current.SetBool(CALIBRATOR_SAVE_STRING, true);

        RemoveTimeCalibrator();

        MoveCalibratorToCorrectPosition();

        ParticleManager.SpawnParticle(ParticleType.SmokePoof, timeCalibratorFinalPosition.position, timeCalibratorFinalPosition);
        AudioManager.Play("UI Click", timeCalibratorFinalPosition);
    }

    private void RemoveTimeCalibrator()
    {
        if (PlayerInventory.GetCurrentItem() == timeCalibratorItem)
        {
            PlayerInventory.RemoveAndDestroyItem();
        }
        else
        {
            Debug.LogError("Trying to remove time calibrator, but it is not the current item in inventory!");
        }
    }

    private void MoveCalibratorToCorrectPosition()
    {
        // timeCalibrator.transform.position = timeCalibratorFinalPosition.position;
        // timeCalibrator.transform.parent = timeCalibratorFinalPosition;
        // timeCalibrator.SetCollider(true);
        timeCalibratorItem.gameObject.SetActive(false);
        timeCalibratorStatic.SetActive(true);

        animator.Play("On");
    }
}