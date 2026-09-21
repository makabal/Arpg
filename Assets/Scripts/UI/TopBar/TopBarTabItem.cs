using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TopBarPage
{
    Character,
    Inventory,
    Skill,
    Quest,
    Map,
    Settings
}

public class TopBarTabItem : MonoBehaviour
{
    [field: SerializeField]
    public TopBarPage Page { get; private set; }
}
