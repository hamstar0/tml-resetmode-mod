using HamstarHelpers.Components.Network;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetProtocol : PacketProtocolRequestToClient {
		private PlayerResetProtocol() { }

		protected override void InitializeClientSendData() { }

		////////////////

		protected override bool ReceiveRequestWithClient() {
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.PromptReset( Main.LocalPlayer );

			return false;
		}

		////

		protected override void ReceiveReply( int fromWho ) { }
	}
}
