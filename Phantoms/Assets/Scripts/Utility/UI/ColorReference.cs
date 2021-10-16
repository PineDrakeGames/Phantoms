using System;
using UnityEngine;

// From here: https://answers.unity.com/questions/1734250/how-to-create-shared-color-channels.html

[CreateAssetMenu(menuName = "Phantoms/Color Reference")]
public class ColorReference : ScriptableObject
{
    [SerializeField] private UnityEngine.Color color;

    public UnityEngine.Color Color { get { return color; } }

    public event Action Changed;

    public float a
    {
        get => color.a;
        set => color.a = value;
    }

    public float r
    {
        get => color.r;
        set => color.r = value;
    }

    public float g
    {
        get => color.g;
        set => color.g = value;
    }

    public float b
    {
        get => color.b;
        set => color.b = value;
    }

    // Define every other function / property you need

    public void Set(UnityEngine.Color color)
    {
        this.color = color;
        Changed?.Invoke();
    }

    public static implicit operator UnityEngine.Color(ColorReference color)
        => color.color;

    public static implicit operator ColorReference(UnityEngine.Color color)
        => new ColorReference() { color = color };
}