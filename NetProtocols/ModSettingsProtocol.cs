using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ModSettingsProtocol : PacketProtocol {
		public ResetModeConfigData Data;


		////////////////

		private ModSettingsProtocol( PacketProtocolDataConstructorLock ctor_lock ) { }

		protected override void SetServerDefaults() {
			this.Data = ResetModeMod.Instance.Config;
		}

		////////////////

		protected override void ReceiveWithClient() {
			var mymod = ResetModeMod.Instance;

			mymod.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}
}
