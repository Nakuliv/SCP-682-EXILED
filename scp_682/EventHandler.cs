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


namespace scp_682
{
    public class EventHandler
    {
        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        public static List<string> scp682 = new List<string>();

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
                int d = new Random().Next(0, 100);
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
            if (scp682.Contains(ev.Attacker.UserId) && ev.Target.Team != Team.SCP && ev.DamageType != DamageTypes.Nuke && ev.DamageType != DamageTypes.Wall && ev.DamageType != DamageTypes.Tesla)
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
                int s = new Random().Next(0, 100);
                if (s <= SCP682.Singleton.Config.spawn_chance)
                {
                    ev.Player.MaxHealth = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Health = SCP682.Singleton.Config.MaxHP;
                    ev.Player.Broadcast(SCP682.Singleton.Config.spawn_message_duration, SCP682.Singleton.Config.spawn_message);
                    ev.Player.Scale = new UnityEngine.Vector3(1.30f, 1, 1.50f);
                    scp682.Add(ev.Player.UserId);
                    coroutines.Add(Timing.RunCoroutine(HealSCP682()));
                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Player.SetRank("", "default");
                    });
                    Timing.CallDelayed(0.5f, () =>
                    {
                        ev.Player.RankName = "SCP-682";
                        ev.Player.RankColor = "red";
                    });
                }
            }
            else if (scp682.Contains(ev.Player.UserId))
            {
                scp682.Remove(ev.Player.UserId);
                ev.Player.RefreshTag();
                ev.Player.Scale = new UnityEngine.Vector3(1, 1, 1);
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
                foreach (Player a in Player.List)
                {
                    if (a.Role == RoleType.Scp93989 && scp682.Contains(a.UserId) && a.Health < SCP682.Singleton.Config.MaxHP)
                    {
                        a.Health = a.Health + SCP682.Singleton.Config.heal_hp;
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
