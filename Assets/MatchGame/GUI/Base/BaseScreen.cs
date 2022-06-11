using UnityEngine;

public abstract class BaseScreen : MonoBehaviour
{
    public virtual void Hide(bool isHidden)
    {
        gameObject.SetActive(!isHidden);
    }
}
