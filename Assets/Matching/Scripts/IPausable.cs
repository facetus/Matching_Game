using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    void PauseContainer();
    void Resume();
    void CloseContainer();
    void ExitGame();
}
