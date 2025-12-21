using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected virtual CanvasGroup Cg { get; private set; }
    protected virtual TextMeshProUGUI ContentText { get; private set; }
    public abstract Transform InitializeTarget { get; protected set; }

    protected virtual void SetContext()
    {
        Cg = GetComponent<CanvasGroup>();
        ContentText = GetComponentInChildren<TextMeshProUGUI>();
        SetInitTarget();
    }

    public virtual void Init(string itemName = null)
    {
        Cg.alpha = 1.0f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// async를 붙여 사용하세요
    /// </summary>
    public abstract Task PlayAnimation();

    protected abstract void SetInitTarget();

    public virtual bool TryInitTarget()
    {
        Transform initializeTarget = this.InitializeTarget;
        if (initializeTarget)
        {
            this.transform.SetParent(initializeTarget);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void SetContentText(Color c)
    {
        ContentText.color = c;
    }
}
