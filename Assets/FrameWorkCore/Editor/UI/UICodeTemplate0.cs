using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region UICodeTemplate
public partial class UICodeTemplate : UIBase
{
    #region multiple member class
    [System.Serializable]
    public class MultipleObject
    {
        #region member
        public Object memberName;
        #endregion
    }
    #endregion
    #region single member
    [SerializeField]
    protected Object memberName;
    #endregion
}
#endregion
