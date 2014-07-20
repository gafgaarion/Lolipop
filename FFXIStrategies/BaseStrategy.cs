using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;
using FFXIWorldKnowledge;
using FFXIWorldKnowledge.Impl;
using FFXIWorldKnowledge.Shards;
using System.Windows.Forms;
using System.Threading;
using CommunicationHandler;
using FFXICommands.Commands;
using FFXIEvents.Events;
using FFXIAggregateRoots;

namespace FFXIStrategies
{
    /// <summary>
    /// Implements command completed events to be aware when a command is finished executing.
    /// </summary>
    public abstract class BaseStrategy : IHandleEvent<CharacterAvailableForActionEvent>, // Generic to all commands
                                         IHandleEvent<CharacterIsBusyWithActionEvent>    // Generic to all commands
    {
        protected List<ConfigurationField> configurations;
        protected ITruthRepository worldTruth;

        // Threading
        protected Thread memoryThread = null;
        protected bool stop;
        private IBus bus;

        // Events-commands variables
        private bool waitingForCommandCompletion = false;
        private bool CommandResult = false;

        abstract protected void Strategy();

        #region Handlers

        virtual public void Handle(CharacterIsBusyWithActionEvent domainEvent)
        {
            sentSwitch = true;
        }

        virtual public void Handle(CharacterAvailableForActionEvent domainEvent)
        {
            waitingForCommandCompletion = false;
            CommandResult = domainEvent.isSuccess;
        }

        #endregion

        public BaseStrategy(ITruthRepository repository, IBus bus)
        {
            this.bus = bus;
            stop = false;
            configurations = new List<ConfigurationField>();
            worldTruth = repository;
        }

        public void ApplyStrategy()
        {
            this.memoryThread = new Thread(StrategyThread);
            this.memoryThread.Start();
        }

        public void Terminate()
        {
            foreach ( CharacterAggregateRoot character in this.worldTruth.getCharacters())
            {
                character.StopMove();
                character.Terminate();
            }

            stop = true;
            if (this.memoryThread != null)
                this.memoryThread.Abort();
        }

        protected void StrategyThread()
        {

            Thread.Sleep(1000);
            while (!stop)
            {
                if (this.worldTruth.getBotLeader() != null && this.worldTruth.charactersAvailable())
                    Strategy();
                Thread.Yield();
            }
        }

        #region BusHandler

        // Sends a command and waits to be notified that it has been received
        private bool sentSwitch = false;
        protected void sendCommand<T>(T command) where T : BaseCommand
        {
            sentSwitch = false;
            DateTime Start = DateTime.Now;

            bus.Send(command);

            while (!sentSwitch && !stop &&
                   ((DateTime.Now - Start).TotalMilliseconds) < 20000) 
                   Thread.Yield();

        }

        protected void sendAsyncCommand<T>(T command) where T : BaseCommand
        {
            sendCommand(command);
        }

        protected void sendAsyncCommandNoOverride<T>(T command) where T : BaseCommand
        {
            if (!this.worldTruth.getAggregateById<CharacterAggregateRoot>(command.characterName).busy)
            {
                sendCommand(command);
            }
            Thread.Sleep(50);
            Thread.Yield();
        }

        /// <summary>
        /// Sends a command and waits for the command the be completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">Command to send to character aggregate</param>
        /// <param name="timeout">Wait timeout in milliseconds</param>
        /// <returns>Returns if command has succeeded</returns>
        protected bool sendAndWaitCompletionCommand<T>(T command, int timeout = -1) where T : BaseCommand
        {
             DateTime Start = DateTime.Now;

            waitingForCommandCompletion = true;
            sendCommand(command);

            Thread.Yield();
            Thread.Sleep(200);
            // Wait till command is done

            return this.waitForCharacter(command.characterName, timeout);
        }

        /// <summary>
        /// Waits for all characters to be freed
        /// </summary>
        /// <param name="_collection"></param>
        /// <param name="timeout"></param>
        /// <returns>Returns a list of characterName of those who have failed</returns>
        protected bool waitForCharacter(string characterName, int timeout = -1)
        {
            List<int> fails = new List<int>();
            DateTime Start = DateTime.Now;

            Thread.Yield();
            Thread.Sleep(200);
            // Wait till command is done
            while ((timeout < 0 || ((DateTime.Now - Start).TotalMilliseconds) < timeout) &&
                    this.worldTruth.getAggregateById<CharacterAggregateRoot>(characterName).busy &&
                    !stop)
            {
                Thread.Sleep(50);
                Thread.Yield();
            }

            if (this.worldTruth.getAggregateById<CharacterAggregateRoot>(characterName).busy)
                return false;

            Thread.Sleep(10);

            bool actionSuccess = this.worldTruth.getAggregateById<CharacterAggregateRoot>(characterName).lastActionSuccess;
            return actionSuccess;
        }

        /// <summary>
        /// Waits for all characters to be freed
        /// </summary>
        /// <param name="_collection"></param>
        /// <param name="timeout"></param>
        /// <returns>Returns a list of characterName of those who have failed</returns>
        protected List<string> waitForCharacters(IEnumerable<CharacterAggregateRoot> _collection, int timeout = -1)
        {
            List<string> fails = new List<string>();
            DateTime Start = DateTime.Now;
            bool allDone = false;

            Thread.Yield();
            Thread.Sleep(200);
            // Wait till command is done
            while ((timeout < 0 || ((DateTime.Now - Start).TotalMilliseconds) < timeout) &&
                    !allDone &&
                    !stop)
            {
                bool localAllDone = true;
                foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
                {
                    if (character.busy)
                        localAllDone = false;
                }
                if (localAllDone)
                    allDone = true;
                Thread.Sleep(50);
                Thread.Yield();
            }

            if (!allDone)
            {
                foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
                {
                    if (character.busy)
                        fails.Add(character.characterName);
                    else
                    {
                        if (!character.lastActionSuccess)
                            fails.Add(character.characterName);
                    }
                }
            }
            return fails;
        }

        /// <summary>
        /// Sends a command to all characters but the leader and waits for the command the be completed for everyone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">Command to send to character aggregate</param>
        /// <param name="timeout">Wait timeout in milliseconds</param>
        /// <returns>Returns a list of characterName of those who have failed</returns>
        protected List<string> broadcastButLeaderAndWaitCompletionCommand<T>(T command, int timeout = -1) where T : BaseCommand
        {
            foreach (CharacterAggregateRoot character in this.worldTruth.getCharactersButLeader())
            {
                command.characterName = character.characterName;
                sendCommand(command);
            }

            return waitForCharacters(this.worldTruth.getCharactersButLeader(), timeout);
        }

        /// <summary>
        /// Sends a command to all characters and waits for the command the be completed for everyone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">Command to send to character aggregate</param>
        /// <param name="timeout">Wait timeout in milliseconds</param>
        /// <returns>Returns a list of characterName of those who have failed</returns>
        protected List<string> broadcastAndWaitCompletionCommand<T>(T command, int timeout = -1) where T : BaseCommand
        {
            foreach (CharacterAggregateRoot character in this.worldTruth.getCharacters())
            {
                command.characterName = character.characterName;
                sendCommand(command);
            }

            return waitForCharacters(this.worldTruth.getCharacters(), timeout);
        }



        #endregion



    }
}
