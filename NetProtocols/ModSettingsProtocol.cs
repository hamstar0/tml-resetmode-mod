using HamstarHelpers.Components.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ModSettingsProtocol : PacketProtocol {
		public ResetModeConfigData Data;

		////////////////

		public override void SetServerDefaults() {
			this.Data = ResetModeMod.Instance.Config;
		}

		protected override void ReceiveWithClient() {
			ResetModeMod.Instance.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}
}
