﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationHandler;
using Commons;
using FFACETools;
using FFXIAggregateRoots;

namespace FFXICommands.Commands
{
    public class UseRangedAttackCommand : BaseCommand
    {
        public ObjectAggregateRoot obj { get; set; }
        public int timeout { get; set; }
    }
}