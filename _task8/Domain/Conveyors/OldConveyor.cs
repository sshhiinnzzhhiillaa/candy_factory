using System;
using System.Collections.Generic;
using System.Text;

namespace _task8.Domain.Conveyors
{
    public class OldConveyor : ConveyorBase
    {
        public override string Name => "OldConveyor";
        public override int ProcessingTime => base.ProcessingTime*2;
        public override double SuccessProbability => 0.7;
        public override int SugarPerCall => base.SugarPerCall + 10;

    }
}
