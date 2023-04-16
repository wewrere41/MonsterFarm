using UnityEngine;
using Zenject;

public abstract class BaseFacade : MonoBehaviour
{
    #region INJECT

    private protected BaseModel _model;

    [Inject]
    public void Construct(BaseModel baseModel)
    {
        _model = baseModel;
    }

    #endregion

    public Animator Animator => _model.GetAnimator;

    public BaseStatsSO StatsSo => _model.BaseStatsSo;
}