using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ModSettingsProtocol : PacketProtocolRequestToServer {
		public ResetModeConfigData Data;


		////////////////

		private ModSettingsProtocol() { }

		protected override void InitializeServerSendData( int toWho ) {
			this.Data = ResetModeMod.Instance.Config;
		}

		////////////////

		protected override void ReceiveReply() {
			var mymod = ResetModeMod.Instance;

			mymod.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( player, ResetModeMod.Instance, "ResetModePlayer" );

			myplayer.FinishModSettingsSyncWithLocal();
		}
	}
}
