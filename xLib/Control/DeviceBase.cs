using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using xLib.Common;
using xLib.Transceiver;
using xLib.UI;

namespace xLib.Controls
{
    public abstract class DeviceBase : IDevice
    {
        public string Name { get; set; } = "DeviceBase";
        public ViewModelBase ViewModel { get; set; }
        public ITerminal Terminal { get; set; }

        protected Task task_update_states;
        protected CancellationTokenSource task_update_states_token_source;

        protected int UpdateStatePeriod = 800;

        public DeviceBase(ITerminal terminal)
        {
            Terminal = terminal;

            task_update_states_token_source = new CancellationTokenSource();
            task_update_states = Task.Run(StateUpdateTask, task_update_states_token_source.Token);
        }

        protected virtual void StateUpdateHandler(xAction<bool, byte[]> transmitter)
        {

        }

        protected async virtual void StateUpdateTask()
        {
            Stopwatch update_time = new Stopwatch();

            try
            {
                while (true)
                {
                    int time = 0;

                    xAction<bool, byte[]> transmit_action = Terminal?.GetTransmitter();

                    if (transmit_action != null)
                    {
                        update_time.Restart();

                        StateUpdateHandler(transmit_action);

                        update_time.Stop();

                        time += (int)update_time.ElapsedMilliseconds;
                    }

                    int delay = UpdateStatePeriod - time;

                    if (delay > 0)
                    {
                        await Task.Delay(delay, task_update_states_token_source.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                xTracer.Message(ex.ToString());
            }
        }

        public virtual bool ResponseIdentification(xContent content)
        {
            return false;
        }

        public virtual void Dispose()
        {
            if (task_update_states_token_source != null)
            {
                task_update_states_token_source?.Cancel();
                task_update_states_token_source?.Dispose();
                task_update_states_token_source = null;
            }

            ViewModel?.Dispose();
            ViewModel = null;
        }
    }
}
