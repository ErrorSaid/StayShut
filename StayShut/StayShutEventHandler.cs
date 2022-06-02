using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace StayShut
{
    public class StayShutEventHandler
    {
        bool CountdownIsActive = false;
        private readonly StayShut plugin;
        CoroutineHandle Handle;
        public StayShutEventHandler(StayShut plugin) => this.plugin = plugin;

        public void RunWhenRoundStarts()
        {
            Handle = Timing.RunCoroutine(CloseAndLockDoors());
            CountdownIsActive = false;
        }

        public void RunWhenRoundEnded(RoundEndedEventArgs EndedRound)
        {
            Timing.KillCoroutines(Handle);
            CountdownIsActive = false;
        }

        public void RunWhenWarheadIsActive(StartingEventArgs WarheadStarted)
        {
            CountdownIsActive = true;
        }

        public void RunWhenWarheadIsStopped(StoppingEventArgs WarheadStopped)
        {
            CountdownIsActive = false;
        }

        private IEnumerator<float> CloseAndLockDoors()
        {
            while (true)
            {
                if (!CountdownIsActive)
                    yield return Timing.WaitForSeconds(0.1f);
                else
                {
                    yield return Timing.WaitForSeconds(plugin.Config.DoorShutTime);
                    while (true)
                    {
                        foreach (Door door in Door.List)
                        {
                            if (!plugin.Config.DoorsShut.Contains(door.Nametag))
                                continue;

                            if (CountdownIsActive)
                            {
                                door.IsOpen = false;
                                door.Lock(999999f, DoorLockType.Lockdown079);
                            }
                            else
                                door.Unlock();
                        }
                        if (!CountdownIsActive)
                            break;
                        yield return Timing.WaitForSeconds(1f);
                    }
                }
            }
        }
    }
}
