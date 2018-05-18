using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public void AddRewards( Player plr, float rewards ) {
			if( Main.netMode != 2 ) { return; }

			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( plr, out success );
			if( !success ) { return; }

			if( this.Rewards.ContainsKey(pid) ) {
				this.Rewards[pid] += rewards;
			} else {
				this.Rewards[pid] = rewards;
			}
		}
	}
}
