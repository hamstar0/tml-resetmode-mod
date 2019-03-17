using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.TmlHelpers;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetProtocol : PacketProtocolRequestToClient {
		private PlayerResetProtocol() { }

		protected override void InitializeClientSendData() { }

		////////////////

		protected override bool ReceiveRequestWithClient() {
			Player player = Main.LocalPlayer;
			var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( player, ResetModeMod.Instance, "ResetModePlayer" );

			myplayer.Logic.PromptReset( player );

			return false;
		}

		////

		protected override void ReceiveReply( int fromWho ) { }
	}
}
