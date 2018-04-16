using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ResetModeModSettingsProtocol : PacketProtocol {
		public ResetModeConfigData Data;

		////////////////

		public override void SetServerDefaults() {
			this.Data = ResetModeMod.Instance.Config;
		}

		public override void ReceiveOnClient() {
			ResetModeMod.Instance.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}
}
