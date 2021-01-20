using _task8.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace _task8.Messages
{
    public class SugarTakenMessage
    {
        public IConveyor Conveyor { get; init; }

        public int ConveyorIndex { get; init; }
    }
}
