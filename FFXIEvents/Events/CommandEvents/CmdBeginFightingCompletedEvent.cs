﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;

namespace FFXIEvents.Events
{
    public class CmdBeginFightingCompletedEvent : DomainEvent
    {
        public bool isSuccess { get; set; }
    }
}
