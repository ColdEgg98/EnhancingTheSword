using UnityEngine;

public class UIRoleBinder : MonoBehaviour
{
    public EUIRole role;
}

public enum EUIRole
{
    Title,
    subheading,
    Description,
    SoloBody,
    MainImage,
    End
}
