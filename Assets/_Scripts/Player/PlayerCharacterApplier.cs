using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacterApplier : MonoBehaviour
{
    void Start()
    {
        var cm = CharacterManager.Instance;
        if (cm == null) return;   // (e.g. testing Gameplay directly without going through the menu)

        Character selected = cm.Selected;
        if (selected == null || selected.inGameVisual == null) return;  // default hero -> keep base controller

        GetComponent<Animator>().runtimeAnimatorController = selected.inGameVisual;
    }
}