using UnityEngine;
using UnityEngine.UI;

public class MuteButtonBehavior : MonoBehaviour
{
    public bool muted = false;
    public Sprite unmutedSprite;
    public Sprite mutedSprite;
    public void OnButtonPressed()
    {
        if (muted)
        {
            muted = false;
            GetComponent<Image>().sprite = unmutedSprite;
            AudioManager.SetMasterVolume(0);
        }
        else
        {
            muted = true;
            GetComponent<Image>().sprite = mutedSprite;
            AudioManager.SetMasterVolume(1);
        }
    }
}
