using MyRule;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroupSigil", menuName = "Sigil/GroupSigil")]
public class GroupSigil : ScriptableObject
{
    public List<NormalSigilSO> normalSigil;
    public List<SpecialSigalSO> specialSigil;
}
