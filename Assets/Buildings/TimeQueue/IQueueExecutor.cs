using System;

namespace Assets.Buildings.TimeQueue
{
    public interface IQueueExecutor
    {
        void Execute(DateTime time);
        void InitProduction(DateTime time);
        DateTime ConstructionTime { get; }
    }
}
