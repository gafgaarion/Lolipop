using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHandler
{
    public interface ICommunicationHandler
    {
        void OpenClientConnection(int characterName);
        void CloseClientConnection(int characterName);
    }
}
