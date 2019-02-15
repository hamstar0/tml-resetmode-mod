using HamstarHelpers.Components.Errors;
using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using ResetMode.NetProtocols;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		private void OnSingleEnterWorld() {
			this.FinishModSettingsSyncWithLocal();
			this.FinishSessionSyncWithLocal();
		}

		private void OnCurrentClientEnterWorld() {
			PacketProtocolRequestToServer.QuickRequest<ModSettingsProtocol>( -1 );
			PacketProtocol.QuickRequestToServer<SessionProtocol>( -1 );
		}

		private void OnServerConnect( Player player ) {
			this.HasModSettings = true;
			this.HasSessionData = true;
			this.IsSynced = true;
		}


		////////////////

		public void FinishModSettingsSyncWithLocal() {
			this.HasModSettings = true;
			
			this.FinishLocalSync();
		}
		
		public void FinishSessionSyncWithLocal() {
			this.HasSessionData = true;

			this.FinishLocalSync();
		}

		////////////////
		
		public void FinishLocalSync() {
			if( Main.netMode == 2 ) { throw new HamstarException("Server not allowed."); }
			if( !this.HasModSettings || !this.HasSessionData ) { return; }

			if( this.IsSynced ) { return; }
			this.IsSynced = true;

			this.Logic.OnFinishLocalSync( this.player );
		}
	}
}
