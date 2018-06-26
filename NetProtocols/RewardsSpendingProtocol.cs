using HamstarHelpers.Components.Network;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.PlayerHelpers;
using Terraria;


namespace ResetMode.NetProtocols {
	class RewardsSpendingProtocol : PacketProtocol {
		public int FromWho;
		public float PP;


		////////////////

		public override void SetClientDefaults() {
			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( Main.LocalPlayer, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode - RewardsSpendingProtocol.SetClientDefaults - Invalid player UID for " + Main.LocalPlayer.name );
				return;
			}

			this.FromWho = Main.LocalPlayer.whoAmI;

			if( !mymod.Session.Data.PlayerPPSpendings.TryGetValue( pid, out this.PP ) ) {
				LogHelpers.Log( "ResetMode - RewardsSpendingProtocol.SetClientDefaults - Invalid player PP for " + Main.LocalPlayer.name );
			}
		}


		////////////////

		protected override void ReceiveWithServer( int from_who ) {
			var mymod = ResetModeMod.Instance;
			Player player = Main.player[this.FromWho];

			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode.NetProtocols.RewardsSpendingProtocol.ReceiveWithServer - Invalid player UID for " + player.name );
				return;
			}

			mymod.Session.Data.PlayerPPSpendings[pid] = this.PP;
		}
	}
}
