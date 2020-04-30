using System;
using System.Linq;
using InfinityScript;

namespace DeadBalance
{
    public class DeadBalance : BaseScript
    {
        bool sv_autoDeadBalance = false;
        public DeadBalance()
        {
            Call("setDvarIfUninitialized", "sv_autoDeadBalance", "1");
            sv_autoDeadBalance = Call<bool>("getDvarInt", "sv_autoDeadBalance");
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            try
            {
                if (player.GetField<string>("sessionteam") != "none" && player.GetField<string>("sessionteam") != "spectator" && sv_autoDeadBalance)
                {
                    if (Players.Count > 1)
                    {
                        int enemyTeam = Players.Where(x => x.GetField<string>("sessionteam") == attacker.GetField<string>("sessionteam")).Count();
                        int friendlyTeam = Players.Where(x => x.GetField<string>("sessionteam") == player.GetField<string>("sessionteam")).Count();

                        if (enemyTeam > friendlyTeam && enemyTeam - friendlyTeam > 1)
                        {
                            player.SetField("sessionteam", getAnotherTeam(player));
                            player.Notify("menuresponse", "team_marinesopfor", getAnotherTeam(player));
                        }
                    }
                }
            } catch (Exception ex)
            {
                Error(ex.ToString());
            }
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }

        private string getAnotherTeam(Entity player)
        {
            if (player.GetField<string>("sessionteam") == "spectator" || player.GetField<string>("sessionteam") == "none")
                return player.GetField<string>("sessionteam");
            return player.GetField<string>("sessionteam") == "allies" ? "axis" : "allies";
        }

        private void Error(string message)
        {
            _(LogLevel.Error, message);
        }
        private void Info(string message)
        {
            _(LogLevel.Info, message);
        }
        private void _(LogLevel logLevel, string message)
        {
            Log.Write(logLevel, message);
        }
    }
}
