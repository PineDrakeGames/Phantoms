using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{

    ////////////////////////
    /// Serialize Fields ///
    ////////////////////////

    [Header("Bottom Bar Notification References")]
    [SerializeField]
    private GameObject m_bottomBarNotificationParent = null;

    [SerializeField]
    private GameObject m_bottomBarImageAndTextParent = null;
    [SerializeField]
    private Image m_bottomBarImageAndTextImage = null;
    [SerializeField]
    private TextMeshProUGUI m_bottomBarImageAndTextText = null;

    [SerializeField]
    private GameObject m_bottomBarTextOnlyParent = null;
    [SerializeField]
    private TextMeshProUGUI m_bottomBarImageTextOnlyText = null;

    /////////////////////////////
    /// Static Instance Stuff ///
    /////////////////////////////
    private static NotificationManager s_instance = null;
    public static NotificationManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<NotificationManager>();
                if (s_instance != null)
                {
                    s_instance.Initialize();
                }
            }
            return s_instance;
        }
    }

    /////////////////////////
    /// Private Variables ///
    /////////////////////////

    private float m_currentDisplayDuration = 0f;
    private const float BOTTOM_BAR_MAX_DISPLAY_DURATION = 8f;

    private bool m_displayingBottomBar = false;


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            Initialize();
        }
    }
    
    private void Update()
    {
        if (m_displayingBottomBar)
        {
            m_currentDisplayDuration += Time.unscaledDeltaTime;
            if (m_currentDisplayDuration >= BOTTOM_BAR_MAX_DISPLAY_DURATION)
            {
                HideNotificationInternal();
            }
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    public static void SetBottomNotification(Sprite image, string text, bool pauseGame = false)
    {
        if (Instance != null) { Instance.SetBottomNotificationInternal(image, text, pauseGame); }
    }

    public static void SetBottomNotification(string text, bool pauseGame = false)
    {
        if (Instance != null) { Instance.SetBottomNotificationInternal(null, text, pauseGame); }
    }

    public static void HideNotification()
    {
        if (Instance != null) { Instance.HideNotificationInternal(); }
    }

    /////////////////////////
    /// Private Functions ///
    /////////////////////////

    private void Initialize()
    {
        HideNotificationInternal();
    }

    private void SetBottomNotificationInternal(Sprite image, string text, bool pauseGame)
    {
        // Just in case, if the notification would have no text, just don't bother.
        if (string.IsNullOrEmpty(text)) { return; }

        // Turn on the notification parent
        if (m_bottomBarNotificationParent) { m_bottomBarNotificationParent.SetActive(true); }

        // Determine whether or not we're setting up the image and text, or just the text.
        if (image != null)
        {
            if (m_bottomBarImageAndTextParent) { m_bottomBarImageAndTextParent.SetActive(true); }
            if (m_bottomBarTextOnlyParent) { m_bottomBarTextOnlyParent.SetActive(false); }

            m_bottomBarImageAndTextImage.sprite = image;
            m_bottomBarImageAndTextText.text = text;
        }
        else
        {
            if (m_bottomBarImageAndTextParent) { m_bottomBarImageAndTextParent.SetActive(false); }
            if (m_bottomBarTextOnlyParent) { m_bottomBarTextOnlyParent.SetActive(true); }
            m_bottomBarImageTextOnlyText.text = text;
        }

        
        if (pauseGame)
        {
            // TODO: Pause the game if we should!
        }

        // Set up stuff for display duration
        m_displayingBottomBar = true;
        m_currentDisplayDuration = 0f;
    }

    public void HideNotificationInternal()
    {
        if (m_bottomBarNotificationParent) { m_bottomBarNotificationParent.SetActive(false); }
        if (m_bottomBarImageAndTextParent) { m_bottomBarImageAndTextParent.SetActive(false); }
        if (m_bottomBarTextOnlyParent) { m_bottomBarTextOnlyParent.SetActive(false); }

        m_displayingBottomBar = false;
    }
}
