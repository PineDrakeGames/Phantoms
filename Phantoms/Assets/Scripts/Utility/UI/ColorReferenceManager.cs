using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Color Reference Manager")]
public class ColorReferenceManager : ScriptableObject
{
    public enum ColorShade
    {
        DARK,
        LIGHT
    }

    [System.Serializable]
    public class ColorScheme
    {
        public ColorReference Dark;
        public ColorReference Light;
    }

    [System.Serializable]
    public class TypeColorScheme : ColorScheme
    {
        public PhantomType Type = PhantomType.NONE;
    }

    public List<TypeColorScheme> TypeColors = new List<TypeColorScheme>();

    public ColorReference GetColor(PhantomType type, ColorShade shade)
    {
        foreach (TypeColorScheme typeColor in TypeColors)
        {
            if (typeColor.Type == type)
            {
                switch (shade)
                {
                    case ColorShade.DARK:
                        return typeColor.Dark;
                    case ColorShade.LIGHT:
                    default:
                        return typeColor.Light;
                }
            }
        }
        return null;
    }
}
