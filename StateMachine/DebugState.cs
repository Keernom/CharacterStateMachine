using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugState : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    void Update()
    {
        _text.text = GetComponent<MovementStateMachine>().MovementSM.CurrentState.ToString();
    }
}
