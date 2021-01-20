using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _task8.Domain
{

    public class Factory
    {
        private object sugarLocker = new object();
        public List<IConveyor> Conveyors { get; set; }
        public int SugarCount { get; set; }

        public bool TakeSugar(IConveyor conveyor)
        {
            lock (sugarLocker)
            {
                if (SugarCount < conveyor.SugarPerCall)
                    return false;
                SugarCount -= conveyor.SugarPerCall;
                OnSugarChanged?.Invoke(conveyor);
                return true;
            }
        }

        public void AddConveyor(IConveyor conveyor)
        {
            Conveyors.Add(conveyor);
            conveyor.Start(this);
        }

        public void DeleteConveyor(IConveyor conveyor)
        {
            Conveyors.Remove(conveyor);
            conveyor.Stop();
        }

        public event Action<IConveyor> OnSugarChanged;

    }

}