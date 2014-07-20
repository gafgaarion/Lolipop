using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface ICommunicationEncoder
    {
        string EncodeMessage(int messageType, Dictionary<string, object> context);
    }
}
