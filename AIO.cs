using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy;

namespace AIO
{
    [ApiVersion(1, 16)]
    public class CawAIO : TerrariaPlugin
    {
        public int actionType = 0;
        private Config config;
        public DateTime LastCheck = DateTime.UtcNow;
        public Players[] Playerlist = new Players[256];

        public override Version Version
        {
            get { return new Version("2.0.0.0"); }
        }

        public override string Name
        {
            get { return "AIO"; }
        }

        public override string Author
        {
            get { return "SpecialOps0 + Terrabear"; }
        }

        public override string Description
        {
            get { return "All-In-One Commands"; }
        }

        public CawAIO(Main game)
            : base(game)
        {
            Order = 1;
        }
    
        #region Initialize
        public override void Initialize()
        {
            //TShockAPI.Commands.ChatCommands.Add(new Command("aio.smack", Smack, "smack"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("aio.bunny", Bunny, "bunny"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.owncast", Owncast, "oc"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.reload", Reload_Config, "aioload"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.rptp", RandomTp, "randomtp", "rptp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.rmtp", RandomMapTp, "randommaptp", "rmtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.igamble", Gamble, "igamble", "ig"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.mgamble", MonsterGamble, "mgamble", "mg"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.jester", Jester, "jester", "jet"));
            TShockAPI.Commands.ChatCommands.Add(new Command("aio.townnpc", TownNpc, "townnpc"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("caw.toggle", toggle, "duckhunttoggle"));

            ServerApi.Hooks.ServerChat.Register(this, NoShadowDodge_Chat);
            ServerApi.Hooks.ServerChat.Register(this, Actionfor);
            ServerApi.Hooks.GameUpdate.Register(this, NoShadowDodge_Armor);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.GameUpdate.Register(this, Cooldowns);
            ReadConfig();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, NoShadowDodge_Chat);
                ServerApi.Hooks.ServerChat.Deregister(this, Actionfor);
                ServerApi.Hooks.GameUpdate.Deregister(this, NoShadowDodge_Armor);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
                ServerApi.Hooks.GameUpdate.Deregister(this, Cooldowns);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Playerlist OnJoin/OnLeave
        public void OnJoin(JoinEventArgs args)
        {
            Playerlist[args.Who] = new Players(args.Who);
        }
        public void OnLeave(LeaveEventArgs args)
        {
            Playerlist[args.Who] = null;
        }
        #endregion

        #region NoShadowDodge(Armor)
        private void NoShadowDodge_Armor(EventArgs e)
        {
            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active && p.ConnectionAlive)
                {
                    for (int i = 0; i < p.TPlayer.buffType.Length; i++)
                    {
                        if (config.NoShadowDodge_Armor && !p.Group.HasPermission("aio.arpass") && p.TPlayer.buffType[i] == 59 && p.TPlayer.buffTime[i] > 20)
                        {
                            p.TPlayer.buffTime[i] = 0;
                            //p.SendErrorMessage("You're not allowed to use shadow dodge!");
                            p.Disable("[AIO] You're not allowed to use shadow dodge!", true);
                        }
                    }
                }
            }
        }
        #endregion

        #region Spawn Town Npcs
        public void TownNpc(CommandArgs args)
        {
            int killcount = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC)
                {
                    TSPlayer.Server.StrikeNPC(i, 99999, 90f, 1);
                    killcount++;
                }
            }
            TSPlayer.All.SendSuccessMessage(string.Format("[NPC] {0} killed {1} town NPCs and respawned them successfully.", args.Player.Name, killcount));
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(19).type, TShock.Utils.GetNPCById(19).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(54).type, TShock.Utils.GetNPCById(54).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(209).type, TShock.Utils.GetNPCById(209).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(38).type, TShock.Utils.GetNPCById(38).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(20).type, TShock.Utils.GetNPCById(20).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(207).type, TShock.Utils.GetNPCById(207).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(107).type, TShock.Utils.GetNPCById(107).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(22).type, TShock.Utils.GetNPCById(22).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(124).type, TShock.Utils.GetNPCById(124).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(17).type, TShock.Utils.GetNPCById(17).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(18).type, TShock.Utils.GetNPCById(18).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(227).type, TShock.Utils.GetNPCById(227).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(208).type, TShock.Utils.GetNPCById(208).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(229).type, TShock.Utils.GetNPCById(229).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(178).type, TShock.Utils.GetNPCById(178).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(353).type, TShock.Utils.GetNPCById(353).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(368).type, TShock.Utils.GetNPCById(368).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(160).type, TShock.Utils.GetNPCById(160).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(228).type, TShock.Utils.GetNPCById(228).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(108).type, TShock.Utils.GetNPCById(108).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            //TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(142).type, TShock.Utils.GetNPCById(142).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
        }
        #endregion

        #region Cooldowns
        private void Cooldowns(EventArgs args)
        {
            if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
            {
                LastCheck = DateTime.UtcNow;
            foreach (var player in Playerlist)
            {
                if (player == null)
                {
                    continue;
                }
                if (player.CD_IGamble > 0)
                {
                    player.CD_IGamble--;
                }
                if (player.CD_MGamble > 0)
                {
                    player.CD_MGamble--;
                }
                if (player.RMTP_Cooldown > 0)
                {
                    player.RMTP_Cooldown--;
                }
                if (player.RPTP_Cooldown > 0)
                {
                    player.RPTP_Cooldown--;
                }
            }
            }
        }
        #endregion

        #region Teleport to random map coordinate
        private void RandomMapTp(CommandArgs args)
        {
            var player = Playerlist[args.Player.Index];

            if (player.RMTP_Cooldown == 0)
            {
                Random rnd = new Random();
                int x = rnd.Next(0, Main.maxTilesX);
                int y = rnd.Next(0, Main.maxTilesY);
                args.Player.Teleport(x * 16, y * 16);
                if (!args.Player.Group.HasPermission("aio.cdpass"))
                {
                    player.RMTP_Cooldown = config.RMTP_Cooldown;
                }
            }
            else
            {
                args.Player.SendErrorMessage("RMTP is on cooldown for {0} seconds.", (player.RMTP_Cooldown));
            }
        }
        #endregion

        #region Teleport to random player
        private void RandomTp(CommandArgs args)
        {
            var player = Playerlist[args.Player.Index];

            if (player.RPTP_Cooldown == 0)
            {

                if (TShock.Utils.ActivePlayers() <= 1)
                {
                    args.Player.SendErrorMessage("[AIO] You cannot teleport to yourself!", Color.Red);
                    return;
                }
                Random rnd = new Random();
                TSPlayer ts = TShock.Players[rnd.Next(0, TShock.Utils.ActivePlayers() - 1)];
                if (!ts.TPAllow && !args.Player.Group.HasPermission("permissions.tpall"))
                {
                    args.Player.SendErrorMessage(ts.Name + " has prevented users from teleporting to him.");
                    ts.SendInfoMessage(args.Player.Name + " attempted to teleport to you.");
                    return;
                }
                args.Player.Teleport(ts.TileX * 16, ts.TileY * 16);

                if (!args.Player.Group.HasPermission("aio.cdpass"))
                {
                    player.RMTP_Cooldown = config.RPTP_Cooldown;
                }
            }
            else
            {
                args.Player.SendErrorMessage("[AIO] RPTP is on cooldown for {0} seconds.", player.RPTP_Cooldown);
            }
        }
        #endregion

        #region Jester
        private void Jester(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendInfoMessage("[Info] /jester(jet) <bird>, <pigstick>, <ball>, <goblin>, <pirate>, <flegion>, <dlegion>");
                args.Player.SendInfoMessage("[Info] /jester(jet) <nmboss>, <mcboss>, <hmboss>, <pumpboss>, <frozboss>, <allboss>");
                return;
            }
            switch (args.Parameters[0])
            {
                case "bird":
                    int amount = 100;
                    int mamount = 74;
                    NPC npcs = TShock.Utils.GetNPCById(mamount);
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned {1} {2} time(s), Tweet tweet!", args.Player.Name, npcs.name, amount), Color.Coral);
                    break;
                case "pigstick":
                    int mamountp = 170;
                    int mamounti = 268;
                    int amountpi = 50;
                    NPC npcsp = TShock.Utils.GetNPCById(mamountp);
                    NPC npcsi = TShock.Utils.GetNPCById(mamounti);
                    TSPlayer.Server.SpawnNPC(npcsp.type, npcsp.name, amountpi, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcsi.type, npcsi.name, amountpi, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned {1} and {2} {3} time(s) each, Bang bang!", args.Player.Name, npcsp.name, npcsi.name, amountpi), Color.Coral);
                    break;
                case "ball":
                    int amount30 = 30;
                    int amount10 = 10;
                    int monsteramountb1 = 25;
                    int monsteramountb2 = 30;
                    int monsteramountb3 = 33;
                    int monsteramountb4 = 70;
                    NPC npcb1 = TShock.Utils.GetNPCById(monsteramountb1);
                    NPC npcb2 = TShock.Utils.GetNPCById(monsteramountb2);
                    NPC npcb3 = TShock.Utils.GetNPCById(monsteramountb3);
                    NPC npcb4 = TShock.Utils.GetNPCById(monsteramountb4);
                    TSPlayer.Server.SpawnNPC(npcb1.type, npcb1.name, amount30, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcb2.type, npcb2.name, amount30, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcb3.type, npcb3.name, amount30, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcb4.type, npcb4.name, amount10, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Dungeon Balls, Pop pop!", args.Player.Name), Color.Coral);
                    break;
                case "allboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(13).type, TShock.Utils.GetNPCById(13).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(50).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(35).type, TShock.Utils.GetNPCById(35).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(4).type, TShock.Utils.GetNPCById(4).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(125).type, TShock.Utils.GetNPCById(125).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(126).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(127).type, TShock.Utils.GetNPCById(127).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(134).type, TShock.Utils.GetNPCById(134).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(266).type, TShock.Utils.GetNPCById(266).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(222).type, TShock.Utils.GetNPCById(222).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(262).type, TShock.Utils.GetNPCById(262).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(245).type, TShock.Utils.GetNPCById(245).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(328).type, TShock.Utils.GetNPCById(328).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(325).type, TShock.Utils.GetNPCById(325).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(344).type, TShock.Utils.GetNPCById(344).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(345).type, TShock.Utils.GetNPCById(345).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(346).type, TShock.Utils.GetNPCById(346).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Every Bosses, Wow Aww!", args.Player.Name), Color.Coral);
                    break;
                case "nmboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(50).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(13).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(35).type, TShock.Utils.GetNPCById(35).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(4).type, TShock.Utils.GetNPCById(4).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(266).type, TShock.Utils.GetNPCById(266).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(222).type, TShock.Utils.GetNPCById(222).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Normal Bosses, Bam bam!", args.Player.Name), Color.Coral);
                    break;
                case "hmboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(125).type, TShock.Utils.GetNPCById(125).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(126).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(127).type, TShock.Utils.GetNPCById(127).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(134).type, TShock.Utils.GetNPCById(134).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(262).type, TShock.Utils.GetNPCById(262).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(245).type, TShock.Utils.GetNPCById(245).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Hardmode Bosses, Boom boom!", args.Player.Name), Color.Coral);
                    break;
                case "pumpboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(328).type, TShock.Utils.GetNPCById(328).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(325).type, TShock.Utils.GetNPCById(325).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(315).type, TShock.Utils.GetNPCById(315).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Pumpkin Bosses, Thunk thunk!", args.Player.Name), Color.Coral);
                    break;
                case "frozboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(344).type, TShock.Utils.GetNPCById(344).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(345).type, TShock.Utils.GetNPCById(345).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(346).type, TShock.Utils.GetNPCById(346).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Frozen Bosses, Jesus Crist!", args.Player.Name), Color.Coral);
                    break;
                case "mcboss":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(125).type, TShock.Utils.GetNPCById(125).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(126).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(127).type, TShock.Utils.GetNPCById(127).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(134).type, TShock.Utils.GetNPCById(134).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Mechanical Bosses, Clink clank!", args.Player.Name), Color.Coral);
                    break;
                case "goblin":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(25).type, TShock.Utils.GetNPCById(25).name, 8, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(26).type, TShock.Utils.GetNPCById(26).name, 5, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(27).type, TShock.Utils.GetNPCById(27).name, 7, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(28).type, TShock.Utils.GetNPCById(28).name, 7, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(29).type, TShock.Utils.GetNPCById(29).name, 5, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has called Goblin Army, Tuck tack!", args.Player.Name), Color.Coral);
                    break;
                case "pirate":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(212).type, TShock.Utils.GetNPCById(212).name, 7, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(213).type, TShock.Utils.GetNPCById(213).name, 6, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(214).type, TShock.Utils.GetNPCById(214).name, 5, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(215).type, TShock.Utils.GetNPCById(215).name, 4, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(216).type, TShock.Utils.GetNPCById(216).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has called Pirates, Mwahahaha!", args.Player.Name), Color.Coral);
                    break;
                case "flegion":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(143).type, TShock.Utils.GetNPCById(143).name, 8, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(144).type, TShock.Utils.GetNPCById(144).name, 8, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(145).type, TShock.Utils.GetNPCById(145).name, 8, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has called Frost Legion! Am I jittery..?", args.Player.Name), Color.Coral);
                    break;
                case "dlegion":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(272).type, TShock.Utils.GetNPCById(272).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(273).type, TShock.Utils.GetNPCById(273).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(277).type, TShock.Utils.GetNPCById(277).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(281).type, TShock.Utils.GetNPCById(281).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(283).type, TShock.Utils.GetNPCById(283).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(285).type, TShock.Utils.GetNPCById(285).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(287).type, TShock.Utils.GetNPCById(287).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(289).type, TShock.Utils.GetNPCById(289).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(290).type, TShock.Utils.GetNPCById(290).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(291).type, TShock.Utils.GetNPCById(291).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(292).type, TShock.Utils.GetNPCById(292).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(293).type, TShock.Utils.GetNPCById(293).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(294).type, TShock.Utils.GetNPCById(294).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has called Dungeon Legion, Kwaaahh!", args.Player.Name), Color.Coral);
                    break;
                case "wof":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(113).type, TShock.Utils.GetNPCById(113).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    args.Player.SendMessage(string.Format("{0} has spawned Wall of Flesh. Oh god, too hot!", args.Player.Name), Color.Coral);
                    break;
                case "king":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(50).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned King slime, Bounce bounce!", args.Player.Name), Color.Coral);
                    break;
                case "golentera":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(245).type, TShock.Utils.GetNPCById(245).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(262).type, TShock.Utils.GetNPCById(262).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendMessage(string.Format("{0} has spawned Golentera, Thump thump! ", args.Player.Name), Color.Coral);
                    break;
            }
        }
        #endregion

        #region Monster Gambling
        private void MonsterGamble(CommandArgs args)
        {
            Random random = new Random();
            int amount = random.Next(1, 50);
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            var selectedPlayer = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.UserAccountName);
            var playeramount = selectedPlayer.BankAccount.Balance;
            var player = Playerlist[args.Player.Index];
            Money moneyamount = -config.MonsterGambleCost;
            Money moneyamount2 = config.MonsterGambleCost;

            if (player.CD_MGamble == 0)
            {
                if (!args.Player.Group.HasPermission("aio.cdpass"))
                {
                    player.CD_MGamble = config.MonsterGambleCooldown;
                }

                if (config.SEconomy)
                {
                    {
                        if (!args.Player.Group.HasPermission("aio.nocost"))
                        {
                            if (playeramount > moneyamount2)
                            {
                                int monsteramount;
                                do
                                {
                                    monsteramount = random.Next(1, Main.maxNPCs);
                                } while (config.MonsterExclude.Contains(monsteramount));

                                NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                                TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                                TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                                args.Player.SendSuccessMessage("You've lost {0} for monster gambling.", moneyamount2);
                                SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, moneyamount, Journalpayment, 
                                    string.Format("{0} has been lost for monster gambling", moneyamount2, args.Player.Name), string.Format("CawAIO: " + "Monster Gambling"));
                            }
                            else
                            {
                                args.Player.SendErrorMessage("You need {0} to gamble, you have {1}.", moneyamount2, selectedPlayer.BankAccount.Balance);
                            }
                        }
                        else
                        {
                            if (args.Player.Group.HasPermission("aio.nocost"))
                            {
                                int monsteramount;
                                do
                                {
                                    monsteramount = random.Next(1, Main.maxNPCs);
                                } while (config.MonsterExclude.Contains(monsteramount));
                                NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                                TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                                TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                                args.Player.SendSuccessMessage("You've lost nothing for monster gambling.");
                            }
                        }
                    }
                }
                else
                {
                    int Randomnpc;

                    do Randomnpc = random.Next(1, Main.maxNPCs);
                    while (config.MonsterExclude.Contains(Randomnpc));

                    NPC npcs = TShock.Utils.GetNPCById(Randomnpc);
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);

                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name,
                        npcs.name, amount));
                }
            }
            else
                    {
                        args.Player.SendErrorMessage("M.gamble is on cooldown for {0} seconds.", player.CD_MGamble);
                    }        
        }
        #endregion

        #region Normal Gambling
        private void Gamble(CommandArgs args)
        {
            Random random = new Random();
            int itemAmount = 0;
            int prefixId = random.Next(1, 83);
            var UsernameBankAccount = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.UserAccountName);
            var playeramount = UsernameBankAccount.BankAccount.Balance;
            var player = Playerlist[args.Player.Index];
            Money amount = -config.GambleCost;
            Money amount2 = config.GambleCost;
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;

            if (player.CD_IGamble == 0)
            {
                if (!args.Player.Group.HasPermission("aio.cdpass"))
                {
                    player.CD_IGamble = config.GambleCooldown;
                }

            if (config.SEconomy)
            {
                int itemName;

                do itemName = random.Next(-48, Main.maxItems);
                while (config.ItemExclude.Contains(itemName));

                Item item = TShock.Utils.GetItemById(itemName);

                if (args.Player != null && UsernameBankAccount != null)
                {
                    itemAmount = random.Next(1, item.maxStack);

                    if (playeramount > amount2)
                    {
                        if (args.Player.InventorySlotAvailable || item.name.ToLower().Contains("Coin"))
                        {
                            if (!args.Player.Group.HasPermission("aio.nocost"))
                            {
                                item.prefix = (byte)prefixId;

                                args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);

                                SEconomyPlugin.WorldAccount.TransferToAsync(UsernameBankAccount.BankAccount, amount,
                                    Journalpayment, string.Format("{0} has been lost for gambling", amount2, args.Player.Name),
                                    string.Format("CawAIO: " + "Item Gambling"));

                                args.Player.SendSuccessMessage("You have lost {0} and gambled {1} {2}(s).", amount2, itemAmount, item.AffixName());

                                Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName());

                                foreach (TSPlayer staffplayer in TShock.Players)
                                {
                                    if (staffplayer != null)
                                        if (staffplayer.Group.HasPermission("caw.gamblevision"))
                                            staffplayer.SendInfoMessage("[I.Gamble] " + args.Player.Name + " has gambled " + itemAmount + " " + item.AffixName());
                                }
                            }
                            else
                            {
                                item.prefix = (byte)prefixId;

                                args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);

                                args.Player.SendSuccessMessage("You've lost nothing and gambled {0} {1}(s).", itemAmount, item.AffixName());

                                Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName());

                                foreach (TSPlayer staffplayer in TShock.Players)
                                {
                                    if (staffplayer != null)
                                    {
                                        if (staffplayer.Group.HasPermission("aio.vgamble"))
                                        {
                                            staffplayer.SendInfoMessage("[I.Gamble] " + args.Player.Name + " has gambled " + itemAmount +
                                                " " + item.AffixName());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Your inventory seems full.");
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("You need {0} to gamble, you have {1}.", amount2, UsernameBankAccount.BankAccount.Balance);
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("The server could not find a valid bank account for the username {0}", args.Player.Name);
                }
            }
            else
            {
                int itemName;
                do
                {
                    itemName = random.Next(-48, Main.maxItems);
                } while (config.ItemExclude.Contains(itemName));

                Item item = TShock.Utils.GetItemById(itemName);

                if (args.Player != null && UsernameBankAccount != null)
                {
                    if (itemAmount > item.maxStack)
                    {
                        itemAmount = item.maxStack;
                    }
                    if (args.Player.InventorySlotAvailable || item.name.Contains("Coin"))
                    {
                        item.prefix = (byte)prefixId;
                        args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);
                        Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName(), Color.Red);

                        foreach (TSPlayer staffplayer in TShock.Players)
                        {
                            if (staffplayer != null)
                            {
                                if (staffplayer.Group.HasPermission("aio.gamblevision"))
                                {
                                    staffplayer.SendMessage("[I.Gamble] " + args.Player.Name + " has gambled " + itemAmount + " " + item.AffixName(), Color.Yellow);
                                }
                            }
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Your inventory seems full.");
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("The server could not find a valid bank account for the username {0}", args.Player.Name);
                }
            }
        }
            else
            {
                args.Player.SendErrorMessage("I.gamble is on cooldown for {0} seconds.", player.CD_IGamble);
            }
        }
        #endregion

        /*#region Smack command
        private void Smack(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string plStr = string.Join(" ", args.Parameters);
                var players = TShock.Utils.FindPlayer(plStr);
                if (players.Count == 0)
                    args.Player.SendErrorMessage("No player matched your query '{0}'", plStr);
                else if (players.Count > 1)
                    TShock.Utils.SendMultipleMatchError(args.Player, players.Select(p => p.Name));
                else
                {
                    var plr = players[0];
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} smacked {1}.",
                                                         args.Player.Name, plr.Name));
                    Log.Info(args.Player.Name + " smacked " + plr.Name + "sub, dude?");
                }
            }
            else
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /smack <player>");
        }
        #endregion*/

        /*private DateTime LastCheck = DateTime.UtcNow;
        private int Duckhunt = 10;
        private bool DuckhuntToggle = config.DuckhuntToggle;
        public void OnUpdatetest(EventArgs args)
        {
            if (DuckhuntToggle && ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1))
            {
                LastCheck = DateTime.UtcNow;
                if (Duckhunt == config.GambleCooldown)
                {
                    TSPlayer.All.SendInfoMessage("This is a test");
                }
                else if (Duckhunt == config.DuckhuntTimer + 10)
                {
                    spawnducks();
                    TSPlayer.All.SendInfoMessage("test");
                }
            }
        }

        private void toggle(CommandArgs args)
        {
            DateTime LastCheck = DateTime.UtcNow;
            DuckhuntToggle = !DuckhuntToggle;
            if (DuckhuntToggle == true || DuckhuntToggle == false)
            {
                args.Player.SendMessage("Duckhunt now: " + ((DuckhuntToggle) ? "Enabled" : "Disabled"), Color.SandyBrown);
            }
        }
        private TShockAPI.DB.Region arenaregion = new TShockAPI.DB.Region();
        private void spawnducks()
        {
            arenaregion = TShock.Regions.GetRegionByName("duckarena");
            int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
            int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
            TSPlayer.All.SendInfoMessage("The ducks fly tonight.");
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(362).type, TShock.Utils.GetNPCById(362).name, 20, arenaX, arenaY, (arenaregion.Area.Width / 2), (arenaregion.Area.Height / 2));

        }*/

        /*#region Bunny Command
        private void Bunny(CommandArgs args)
        {
            TSPlayer player = TShock.Players[args.Player.Index];
            {
                player.SendMessage("[AIO] You've been buffed with a pet bunny!", Color.Green);
                player.SetBuff(40, 60, true);
            }
        }
        #endregion*/

        #region Owncast Command
        private void Owncast(CommandArgs args)
        {
            string message = string.Join(" ", args.Parameters);

            TShock.Utils.Broadcast(
                "[MOTD] " + message, Color.PaleGoldenrod);
        }
        #endregion

        #region NoShadowDodge(Chat) 
        private void NoShadowDodge_Chat(ServerChatEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];
            
            if (args.Handled)
            {
                return;
            }
            
            if (player == null)
            {
                args.Handled = true;
                return;
            }

            if (config.NoShadowDodge_Chat)
            {
                if (args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("\"shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("59"))
                {
                    if (player.Group.HasPermission("aio.bfpass"))
                    {
                        args.Handled = false;
                    }
                    else
                    {
                        args.Handled = true;
                        player.SendErrorMessage("[AIO] You can't use Shadow Dodge buff!");
                    }
                }
            }
        }
        #endregion

        #region Block Banned Words
        private void Actionfor(ServerChatEventArgs args)
        {
            var ignored = new List<string>();
            var censored = new List<string>();
            var player = TShock.Players[args.Who];
            var text = args.Text;

            if (!args.Text.ToLower().StartsWith("/") || args.Text.ToLower().StartsWith("/w") ||
                args.Text.ToLower().StartsWith("/r") || args.Text.ToLower().StartsWith("/me") ||
                args.Text.ToLower().StartsWith("/c") || args.Text.ToLower().StartsWith("/party"))
            {
                foreach (string Word in config.BanWords)
                {
                    if (player.Group.HasPermission("aio.bwpass"))
                    {
                        args.Handled = false;
                    }

                    else if (args.Text.ToLower().Contains(Word))
                    {
                        if (player.mute)
                        {
                            player.SendErrorMessage("[AIO] You are muted!");
                            return;
                        }
                        else
                        {
                            switch (config.ActionForBannedWord)
                            {
                                case "kick":
                                    args.Handled = true;
                                    TShock.Utils.Kick(player, config.KickMessage, true, false);
                                    return;
                                case "ignore":
                                    args.Handled = true;
                                    ignored.Add(Word);
                                    break;
                                case "censor":
                                    args.Handled = true;
                                    text = args.Text;
                                    text = args.Text.Replace(Word, new string('*', Word.Length));
                                    TSPlayer.All.SendMessage("<" + "(" + player.Group.Name + ") " + player.Name + ">" + text, player.Group.R, player.Group.G, player.Group.B);
                                    //TSPlayer.All.SendMessage(player.Group.Prefix + player.Name + ": " + text, player.Group.R, player.Group.G, player.Group.B);
                                    return;
                                case "null":
                                    args.Handled = false;
                                    break;
                            }
                        }
                    }
                }
                if (ignored.Count > 0)
                {
                    player.SendErrorMessage("You've been ignored for saying: " + string.Join(", ", ignored));
                    return;
                }
            }
            else
            {
                args.Handled = false;
            }
        }
        #endregion

        #region Create Config File
        private void CreateConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "AIO.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
        }
        #endregion

        #region Read Config File
        private bool ReadConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "AIO.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("[AIO] Config was not found. Creating a new one..");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }
        #endregion

        #region Config
        public class Config
        {
            public string ActionForBannedWord = "ignore";
            public string[] BanWords = { "yolo", "swag", "fuck", "shit" };
            public string KickMessage = "You've said a banned word.";
            public bool ForceHalloween = false;
            public bool SEconomy = true;
            public bool NoShadowDodge_Chat = false;
            public bool NoShadowDodge_Armor = false;
            //public bool DuckhuntToggle = false
            public int GambleCost = 50000;
            public int GambleCooldown = 1800;
            public int MonsterGambleCost = 50000;
            public int MonsterGambleCooldown = 1800;
            public int RMTP_Cooldown = 60;
            public int RPTP_Cooldown = 60;
            public int[] ItemExclude = { 665, 666, 667, 668, 1131, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568 };
            public int[] MonsterExclude = { 9, 22, 68, 17, 18, 37, 38, 19, 20, 37, 54, 68, 106, 123, 124, 107, 108, 113, 142, 178, 207, 208, 209, 227, 228, 160, 229, 353, 368 };
            
            /*public int MonsterGambleCooldown = 0;
            public int GambleCooldown = 0;
            public int DuckhuntTimer = 10;*/

        }
        #endregion

        #region Reload Config File
        private void Reload_Config(CommandArgs args)
        {
            if (ReadConfig())
            {
                args.Player.SendSuccessMessage("[AIO] Config reloaded.");
            }
            else
            {
                args.Player.SendErrorMessage("[AIO] Failed to reload config. Check logs for details.");
            }
        }
        #endregion
    }

    #region Players
    public class Players
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public int RMTP_Cooldown { get; set; }
        public int RPTP_Cooldown { get; set; }
        public int CD_IGamble { get; set; }
        public int CD_MGamble { get; set; }

        public Players(int index)
        {
            this.Index = index;
        }
    }
    #endregion
}