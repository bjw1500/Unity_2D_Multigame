using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{

    public enum UIEvent
    {
        Click,
        Drag,


    }
    public enum Scene
    {
        Unknown,
        WaitingRoom,
        Lobby,
        Game,
    }

}
