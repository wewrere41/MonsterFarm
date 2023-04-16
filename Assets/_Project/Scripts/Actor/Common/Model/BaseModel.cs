using UnityEngine;

public abstract class BaseModel
{
    protected BaseModel(Animator animator, BaseStatsSO baseStatsSo)
    {
        GetAnimator = animator;
        BaseStatsSo = baseStatsSo;
    }

    public BaseStatsSO BaseStatsSo { get; set; }
    public Animator GetAnimator { get; }
    public GameObject GO => GetAnimator.gameObject;

    public GameObject MeshGO => GO.transform.GetChild(2).gameObject;

    public Vector3 Position
    {
        get => GetAnimator.gameObject.transform.position;
        set => GetAnimator.gameObject.transform.position = value;
    }
}