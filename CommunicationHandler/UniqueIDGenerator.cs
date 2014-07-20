using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CommunicationHandler
{

    public sealed class UniqueIDGenerator
    {
        private static volatile UniqueIDGenerator instance;
        private static object syncRoot = new Object();

        private UniqueIDGenerator() { currentId = 0; }

        private int currentId;

        public static UniqueIDGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UniqueIDGenerator();
                    }
                }

                return instance;
            }
        }

        public int GetUniqueId()
        {
            lock (syncRoot)
            {
                currentId++;
                return currentId;
            }
        }
    }
}
