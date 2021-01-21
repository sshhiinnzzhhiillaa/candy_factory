using System;
using System.Collections.Generic;
using System.Text;

namespace FactoryLibrary.Domain.Conveyors
{
    public class NewConveyor : ConveyorBase
    {
        public override string Name => "NewConveyor";
        public override int ProcessingTime => base.ProcessingTime;
        public override double SuccessProbability => base.SuccessProbability;
        public override int SugarPerCall => base.SugarPerCall;
    }
}
