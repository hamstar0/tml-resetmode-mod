using HamstarHelpers.Utilities.Network;
using Terraria;


namespace ResetMode.NetProtocols {
	class ResetModePlayerResetProtocol : PacketProtocol {
		public override void SetClientDefaults() { }

		protected override bool ReceiveRequestWithClient() {
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.PromptReset( ResetModeMod.Instance, Main.LocalPlayer );

			return true;
		}
	}
}
