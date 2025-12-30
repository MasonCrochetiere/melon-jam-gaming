using UnityEngine;

public class LockPlayerForTime : MonoBehaviour
{
    [SerializeField] float lockTime = 12f;
    [SerializeField] PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.moveActivated = false;
    }

    public void LockPlayer()
    {
        Invoke("UnlockPlayer", lockTime);
    }

    void UnlockPlayer()
    {
        player.moveActivated = true;
    }
}
