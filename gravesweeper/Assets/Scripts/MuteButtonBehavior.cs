using UnityEngine;
using UnityEngine.UI;

public class MuteButtonBehavior : MonoBehaviour
{
    public bool muted = false;
    public GameObject unmutedSprite;
    public GameObject mutedSprite;
    public void OnButtonPressed()
    {
        if (muted)
        {
            muted = false;
            unmutedSprite.SetActive(true);
            mutedSprite.SetActive(false);
            AudioManager.SetMasterVolume(1);
        }
        else
        {
            muted = true;
            mutedSprite.SetActive(true);
            unmutedSprite.SetActive(false);
            AudioManager.SetMasterVolume(0);
        }
    }
}
