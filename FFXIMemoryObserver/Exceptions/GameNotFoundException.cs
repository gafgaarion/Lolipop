using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIMemoryObserver.Exceptions
{
    public class GameNotFoundException : Exception
    {

        public GameNotFoundException(string message = "Game is not running.")
            : base(message)
        { }
    }
}
