using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        SOUND
    }

    [SerializeField]
    private VolumeType m_volumeType = VolumeType.MASTER;

    [SerializeField]
    private AudioClip m_confirmSound = null;

    private Slider m_slider = null;

    public static UnityEvent VolumeUpdate = new UnityEvent();

    private void Awake()
    {
        m_slider = GetComponent<Slider>();
        if (VolumeUpdate == null)
        {
            VolumeUpdate = new UnityEvent();
        }
    }

    private void OnEnable()
    {
        float volume = 0f;
        switch (m_volumeType)
        {
            case VolumeType.MASTER:
                volume = DataManager.Instance.Settings.MasterVolume;
                break;
            case VolumeType.MUSIC:
                volume = DataManager.Instance.Settings.MusicVolume;
                break;
            case VolumeType.SOUND:
                volume = DataManager.Instance.Settings.SoundsVolume;
                break;
        }
        m_slider.value = volume;
        m_slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void OnDestroy()
    {
        if (m_slider)
        {
            m_slider.onValueChanged.RemoveListener(ChangeVolume);
        }
    }

    public void ChangeVolume(float newVolume)
    {

        switch (m_volumeType)
        {
            case VolumeType.MASTER:
                DataManager.Instance.Settings.MasterVolume = newVolume;
                break;
            case VolumeType.MUSIC:
                DataManager.Instance.Settings.MusicVolume = newVolume;
                break;
            case VolumeType.SOUND:
                DataManager.Instance.Settings.SoundsVolume = newVolume;
                break;
        }
        AudioManager.UpdateVolume();
        VolumeUpdate.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // do nothing
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_confirmSound)
        {
            AudioManager.PlaySound(m_confirmSound);
        }
    }

}
