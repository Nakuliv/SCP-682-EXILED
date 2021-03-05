using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace scp_682
{
    public static class Extensions
    {
        internal static bool hasTag;
        internal static bool isHidden;

        public static void SetRank(this Player player, string rank, string color = "default")
        {
            player.ReferenceHub.serverRoles.NetworkMyText = rank;
            player.ReferenceHub.serverRoles.NetworkMyColor = color;
        }

        public static void RefreshTag(this Player player)
        {
            player.ReferenceHub.serverRoles.HiddenBadge = null; player.ReferenceHub.serverRoles.RpcResetFixed(); player.ReferenceHub.serverRoles.RefreshPermissions(true);
        }
    }
}
