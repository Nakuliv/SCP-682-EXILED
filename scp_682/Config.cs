using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace scp_682
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public int spawn_chance { get; set; } = 100;
        public Exiled.API.Features.Broadcast spawn_message { get; private set; } = new Exiled.API.Features.Broadcast("<b>You are <color=red>SCP-682</color></b>", 10);
        [Description("is the SCP-682 supposed to kill with one bite")]
        public bool can_kill_on_oneshot { get; set; } = true;
        [Description("whether SCP-682 is to be able to Pry Gates?")]
        public bool can_PryGates { get; set; } = true;
        [Description("how much hp should SCP-682 get when he damage a human")]
        public int heal_hp_when_eat { get; set; } = 5;
        [Description("max hp")]
        public int MaxHP { get; set; } = 2200;
        [Description("every how many seconds SCP-682 health should regenerate?")]
        public int heal_time { get; set; } = 5;
        [Description("how much hp should be regenerated?")]
        public int heal_hp { get; set; } = 5;
        [Description("can scp 682 destroy doors?")]
        public bool scp682_can_destroy_door { get; set; } = true;
        [Description("how many % chance should SCP-682 have to destroy the door")]
        public int scp682_destroy_door_chance { get; set; } = 100;
    }
}
