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
        public Dictionary<Player, DateTime> BreakDoorCooldowns = new Dictionary<Player, DateTime>();
        public Dictionary<Player, DateTime> PryGateCooldowns = new Dictionary<Player, DateTime>();
        public static List<string> scp682 = new List<string>();
        public System.Random rnd = new System.Random();

        public void OnDoorAccess(InteractingDoorEventArgs ev)
        {
            if (scp682.Contains(ev.Player.UserId) && ev.Door.Base is PryableDoor pdoor)
            {
                if (PryGateCooldowns.ContainsKey(ev.Player))
                {
                    DateTime cooldownTime = PryGateCooldowns[ev.Player] + TimeSpan.FromSeconds(SCP682.Singleton.Config.PryGateCooldown);
                    if (DateTime.Now < cooldownTime)
                    {
                        ev.Player.ShowHint(SCP682.Singleton.Config.PryGateCooldownMessage.Replace("%time%", Math.Round((cooldownTime - DateTime.Now).TotalSeconds, 2).ToString()));
                    }
                    else
                    {
                        PryGateCooldowns.Remove(ev.Player);

                        if (SCP682.Singleton.Config.can_PryGates)
                        {
                            pdoor.TryPryGate();
                            PryGateCooldowns[ev.Player] = DateTime.Now;
                        }
                    }
                }
                else if (SCP682.Singleton.Config.can_PryGates)
                {
                    pdoor.TryPryGate();
                    PryGateCooldowns[ev.Player] = DateTime.Now;
                }
            }
            else if (SCP682.Singleton.Config.scp682_can_destroy_door && scp682.Contains(ev.Player.UserId))
            {
                if (BreakDoorCooldowns.ContainsKey(ev.Player))
                {
                    DateTime cooldownTime = BreakDoorCooldowns[ev.Player] + TimeSpan.FromSeconds(SCP682.Singleton.Config.DestroyDoorCooldown);
                    if (DateTime.Now < cooldownTime)
                    {
                        ev.Player.ShowHint(SCP682.Singleton.Config.DestroyDoorCooldownMessage.Replace("%time%", Math.Round((cooldownTime - DateTime.Now).TotalSeconds, 2).ToString()));
                    }
                    else
                    {
                        BreakDoorCooldowns.Remove(ev.Player);

                        int d = rnd.Next(0, 101);
                        if (d <= SCP682.Singleton.Config.scp682_destroy_door_chance)
                        {
                            ev.Door.BreakDoor();
                            BreakDoorCooldowns[ev.Player] = DateTime.Now;
                        }
                    }
                }
                else
                {
                    int d = rnd.Next(0, 101);
                    if (d <= SCP682.Singleton.Config.scp682_destroy_door_chance)
                    {
                        ev.Door.BreakDoor();
                        BreakDoorCooldowns[ev.Player] = DateTime.Now;
                    }
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
                    ev.Target.CustomInfo = null;
                }
            }
        }

        public void OnPlayerHurt(HurtingEventArgs ev)
        {
            if (scp682.Contains(ev.Attacker.UserId) && ev.Attacker.Team != Team.SCP)
            {
                if (SCP682.Singleton.Config.can_kill_on_oneshot)
                {
                    ev.Target.Kill(DamageTypes.Scp939);
                }
                if (ev.Attacker.Health < SCP682.Singleton.Config.MaxHP)
                {
                    ev.Attacker.Health = ev.Attacker.Health + SCP682.Singleton.Config.heal_hp_when_kill;
                }
            }
        }

        public void OnSetRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Scp93989 && !scp682.Contains(ev.Player.UserId))
            {
                int s = rnd.Next(0, 101);
                if (s <= SCP682.Singleton.Config.spawn_chance)
                {
                    ev.Player.MaxHealth = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Health = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Broadcast(SCP682.Singleton.Config.spawn_message.Duration, SCP682.Singleton.Config.spawn_message.Content);
                    ev.Player.Scale = new Vector3(1.22f, 1, 1.22f);
                    ev.Player.CustomInfo = $"<color={SCP682.Singleton.Config.DisplayColor}>{SCP682.Singleton.Config.DisplayName}</color>";
                    scp682.Add(ev.Player.UserId);
                    coroutines.Add(Timing.RunCoroutine(HealSCP682(ev.Player)));
                }
            }
            else if (scp682.Contains(ev.Player.UserId))
            {
                ev.Player.CustomInfo = null;
                scp682.Remove(ev.Player.UserId);
                ev.Player.Scale = new Vector3(1, 1, 1);
                foreach (CoroutineHandle coroutine in coroutines)
                {
                    Timing.KillCoroutines(coroutine);
                }
                coroutines.Clear();
            }
        }
        public IEnumerator<float> HealSCP682(Player ply)
        {
            while (true)
            {
                if (ply.Role == RoleType.Scp93989 && scp682.Contains(ply.UserId) && (ply.Health < SCP682.Singleton.Config.MaxHP))
                {
                    ply.Health = ply.Health + SCP682.Singleton.Config.heal_hp;
                }
                yield return Timing.WaitForSeconds(SCP682.Singleton.Config.heal_time);
            }
        }
        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            coroutines.Clear();

            scp682.Clear();
        }
    }
}
