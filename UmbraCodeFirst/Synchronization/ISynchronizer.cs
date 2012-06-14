using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UmbraCodeFirst.Synchronization
{
    public interface ISynchronizer
    {
        void Synchronize();
        int ExecutionOrder { get; }
    }
}
