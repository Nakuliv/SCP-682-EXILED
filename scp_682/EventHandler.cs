using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Interactables.Interobjects;
using MEC;
using UnityEngine;

namespace scp_682
{
    public class EventHandler
    {
        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        public static List<string> scp682 = new List<string>();
        public System.Random rnd = new System.Random();

        public void OnDoorAccess(InteractingDoorEventArgs ev)
        {
                if (scp682.Contains(ev.Player.UserId) && ev.Door is PryableDoor pdoor)
            {
                if (SCP682.Singleton.Config.can_PryGates == true)
                {
                    pdoor.TryPryGate();
                }
                }
            else if (SCP682.Singleton.Config.scp682_can_destroy_door == true && scp682.Contains(ev.Player.UserId))
            {
                int d = rnd.Next(0, 100);
                if (d <= SCP682.Singleton.Config.scp682_destroy_door_chance)
                {
                    ev.Door.BreakDoor();
                }
            }
        }

        public void OnPlayerDie(DiedEventArgs ev)
        {
            if (ev.Target.Role == RoleType.Scp93989)
            {
                if (scp682.Contains(ev.Target.UserId))
                {
                    scp682.Remove(ev.Target.UserId);
                    ev.Target.RefreshTag();
                }
            }
        }

        public void OnPlayerHurt(HurtingEventArgs ev)
        {
            if (scp682.Contains(ev.Attacker.UserId))
            {
                if (SCP682.Singleton.Config.can_kill_on_oneshot == true)
                {
                    ev.Target.Kill(DamageTypes.Scp939);
                }
                if (ev.Attacker.Health < SCP682.Singleton.Config.MaxHP)
                {
                    ev.Attacker.Health = ev.Attacker.Health + SCP682.Singleton.Config.heal_hp_when_eat;
                }
            }
        }

        public void OnSetRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Scp93989 && !scp682.Contains(ev.Player.UserId))
            {
                int s = rnd.Next(0, 100);
                if (s <= SCP682.Singleton.Config.spawn_chance)
                {
                    ev.Player.MaxHealth = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Health = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Broadcast(SCP682.Singleton.Config.spawn_message.Duration  , SCP682.Singleton.Config.spawn_message.Content);
                    ev.Player.Scale = new Vector3(1.30f, 1, 1.50f);
                    ev.Player.CustomInfo = "<color=red>SCP-682</color>";
                    scp682.Add(ev.Player.UserId);
                    coroutines.Add(Timing.RunCoroutine(HealSCP682()));
                }
            }
            else if (scp682.Contains(ev.Player.UserId))
            {
                ev.Player.CustomInfo = null;
                scp682.Remove(ev.Player.UserId);
                ev.Player.RefreshTag();
                ev.Player.Scale = new Vector3(1, 1, 1);
                foreach (CoroutineHandle coroutine in coroutines)
                {
                    Timing.KillCoroutines(coroutine);
                }
                coroutines.Clear();
            }
        }
        public IEnumerator<float> HealSCP682()
        {
            for (; ; )
            {
                foreach (Player ply in Player.List)
                {
                    if (ply.Role == RoleType.Scp93989 && scp682.Contains(ply.UserId) && ply.Health < SCP682.Singleton.Config.MaxHP)
                    {
                        ply.Health = ply.Health + SCP682.Singleton.Config.heal_hp;
                    }
                }
                yield return Timing.WaitForSeconds(SCP682.Singleton.Config.heal_time);
            }
        }
        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            scp682.Clear();
        }
    }
}
