using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterStateBase : BaseState<MonsterBase>
{
    protected MonsterStateBase(StateHandler<MonsterBase> handler) : base(handler) { }
}