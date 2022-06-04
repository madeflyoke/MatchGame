using MatchGame.Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class BaseButton : MonoBehaviour
{
    [Inject] protected GameManager gameManager;

    public Button Button { get; protected set; }

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
    }

    protected virtual void OnEnable()
    {
        Button.onClick.AddListener(Listeners);
    }
    protected virtual void OnDisable()
    {
        Button.onClick.RemoveListener(Listeners);
    }

    public abstract void Listeners();
}
