using HamstarHelpers.Utilities.Network;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetConfirmProtocol : PacketProtocol {
		public override void SetServerDefaults() { }

		protected override bool ReceiveRequestWithServer( int from_who ) {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.player[ from_who ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSession( mymod, player );
			myplayer.Logic.ResetRewards( mymod, player );

			return true;
		}
	}
}
