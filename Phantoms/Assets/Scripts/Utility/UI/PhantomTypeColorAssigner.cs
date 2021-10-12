using UnityEngine;

[ExecuteInEditMode]
public abstract class PhantomTypeColorAssigner : MonoBehaviour
{
    [SerializeField] private ColorReferenceManager m_colorManager;

    [SerializeField] private PhantomType m_type = PhantomType.NONE;
    public PhantomType Type
    {
        get { return m_type; }
        set
        {
            m_type = value;
            OnColorChanged();
        }
    }

    [SerializeField]
    private ColorReferenceManager.ColorShade m_shade = ColorReferenceManager.ColorShade.DARK;
    public ColorReferenceManager.ColorShade Shade
    {
        get { return m_shade; }
        set
        {
            m_shade = value;
            OnColorChanged();
        }
    }

    public ColorReference color
    {
        get
        {
            if (m_colorManager != null)
            {
                return m_colorManager.GetColor(m_type, m_shade);
            }
            return null;
        }
    }

    [SerializeField]
    [HideInInspector]
    private ColorReference m_prevColor = null;

    protected abstract void AssignColor(Color color);

    private void SubscribeColorChange() => color.Changed += OnColorChanged;

    private void UnsubscribeColorChange() => color.Changed -= OnColorChanged;

    private void OnColorChanged()
    {
        if (color != m_prevColor)
        {
            m_prevColor = color;
        }
        AssignColor(color);
    }

    private void Start()
    {
        if (color != null)
        {
            UnsubscribeColorChange();
            SubscribeColorChange();
            AssignColor(color);
        }
    }

    private void OnDestroy()
    {
        if (color != null)
            UnsubscribeColorChange();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_colorManager != null && color != m_prevColor)
        {
            UnsubscribeColorChange();
            SubscribeColorChange();
            OnColorChanged();
        }
    }
#endif

}