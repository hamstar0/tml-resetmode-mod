using HamstarHelpers.Classes.Errors;
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
			SessionProtocol.SyncToMe();
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
			if( Main.netMode == 2 ) { throw new ModHelpersException("Server not allowed."); }
			if( !this.HasModSettings || !this.HasSessionData ) { return; }

			if( this.IsSynced ) { return; }
			this.IsSynced = true;

			this.Logic.OnFinishLocalSync( this.player );
		}
	}
}
