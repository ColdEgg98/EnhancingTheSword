using System.Threading.Tasks;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public abstract void Init(string itemName);
    public abstract Task PlayAnimation();
    public abstract Transform initializeTarget { get; protected set;}
    public virtual bool TryInitTarget()
    {
        Transform initializeTarget = this.initializeTarget;
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
}
