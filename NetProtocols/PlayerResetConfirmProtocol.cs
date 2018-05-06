using HamstarHelpers.Utilities.Network;
using Terraria;


namespace ResetMode.NetProtocols {
	class ResetModePlayerResetConfirmProtocol : PacketProtocol {
		public override void SetServerDefaults() { }

		protected override bool ReceiveRequestWithServer( int from_who ) {
			Player player = Main.player[ from_who ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSession( ResetModeMod.Instance, player );

			return true;
		}
	}
}
