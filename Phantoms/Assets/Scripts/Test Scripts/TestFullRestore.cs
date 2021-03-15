using UnityEngine;

public class TestFullRestore : MonoBehaviour
{
    public void FullRestore()
    {
        DataManager.Instance.FullHeal();
    }
}
