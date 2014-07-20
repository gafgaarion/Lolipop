using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface IClientCommunicationInterpreter
    {
        void Interpret(byte[] rawStruct);
    }
}
