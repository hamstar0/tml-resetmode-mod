using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetProtocol : PacketProtocol {	//PacketProtocolRequestToClient {
		protected PlayerResetProtocol( PacketProtocolDataConstructorLock ctor_lock ) : base( ctor_lock ) { }

		protected override void SetClientDefaults() { }

		////////////////

		protected override bool ReceiveRequestWithClient() {
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.PromptReset( ResetModeMod.Instance, Main.LocalPlayer );

			return true;
		}
	}
}
