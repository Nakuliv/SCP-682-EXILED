using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System.Reflection;

using PlayerEv = Exiled.Events.Handlers.Player;
using ServerEv = Exiled.Events.Handlers.Server;
using MEC;

namespace scp_682
{
    public class SCP682 : Plugin<Config>
    {
        public override string Name { get; } = "SCP-682";
        public override string Author { get; } = "Naku";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion => new Version(3, 0, 0);

        private EventHandler handler;

        public override void OnEnabled()
        {
            handler = new EventHandler();
            SCP682.Singleton = this;
            ServerEv.RoundEnded += handler.OnRoundEnd;
            PlayerEv.ChangingRole += handler.OnSetRole;
            PlayerEv.Hurting += handler.OnPlayerHurt;
            PlayerEv.Died += handler.OnPlayerDie;
            PlayerEv.InteractingDoor += handler.OnDoorAccess;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEv.RoundEnded -= handler.OnRoundEnd;
            PlayerEv.ChangingRole -= handler.OnSetRole;
            PlayerEv.Hurting -= handler.OnPlayerHurt;
            PlayerEv.Died -= handler.OnPlayerDie;
            PlayerEv.InteractingDoor -= handler.OnDoorAccess;
            handler = null;

            base.OnDisabled();
        }
        public static SCP682 Singleton;
    }
}
