using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMemberMark : MonoBehaviour
{
    [System.Serializable]
    public class MemberInfo
    {
        public UnityEngine.Object memberObj;
        public string memberName;
    }

    public enum QuantityMode
    {
        Single = 0,
        Multiple = 1,
    }

    public QuantityMode quantityMode;
    #region single
    public MemberInfo singleMemberInfo;
    #endregion

    #region multiple
    public bool bUseGroup;
    public string groupName;
    public List<MemberInfo> multipleMemberInfo;
    #endregion
}