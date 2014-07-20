using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;
using FFXIWorldKnowledge;

namespace FFXIStrategies
{
    public interface IStrategy
    {
        void ApplyStrategy();
        void Terminate();
        Type getType();
        List<ConfigurationField> getConfigurationFields();
    }
}
