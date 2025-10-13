using UnityEngine;
public interface IStrategyContext { }
public interface IStrategy
{
    void Execute(IStrategyContext context);
}