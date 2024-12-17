using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCState
{
    public void Enter();
    public void Exit();
    public void OnUpdate();
}
