using UnityEngine;

[ExecuteInEditMode]
public abstract class ColorAssigner : MonoBehaviour
{
    [SerializeField] private ColorReference color;

    public ColorReference ColorReference
    {
        get => color;
        set
        {
            if (color == value)
                return;

            ColorReference oldValue = color;

            if (oldValue != null && value == null)
                UnsubscribeColorChange();

            color = value;

            if (oldValue == null && value != null)
                SubscribeColorChange();
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
        if (color != null)
        {
            UnsubscribeColorChange();
            SubscribeColorChange();
            OnColorChanged();
        }
    }
#endif
}