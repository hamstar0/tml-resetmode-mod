using HamstarHelpers.Components.Errors;
using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.NetProtocols;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		private void OnSingleConnect() {
			this.FinishModSettingsSync();
			this.FinishSessionSync();
		}

		private void OnCurrentClientConnect() {
			PacketProtocol.QuickRequestToServer<ModSettingsProtocol>();
			PacketProtocol.QuickRequestToServer<SessionProtocol>();
		}

		private void OnServerConnect( Player player ) {
			this.HasModSettings = true;
			this.HasSessionData = true;
			this.IsSynced = true;
		}


		////////////////

		public void FinishModSettingsSync() {
			this.HasModSettings = true;

			this.FinishLocalSync();
		}
		
		public void FinishSessionSync() {
			this.HasSessionData = true;

			this.FinishLocalSync();
		}

		////////////////
		
		public void FinishLocalSync() {
			if( Main.netMode == 2 ) { throw new HamstarException("Server not allowed."); }
			if( !this.HasModSettings || !this.HasSessionData ) { return; }
			if( this.IsSyncing ) { return; }

			this.IsSyncing = true;
			
			Promises.AddWorldInPlayOncePromise( () => {
				this.IsSynced = true;
				this.Logic.OnEnterWorld( (ResetModeMod)this.mod, this.player );
			} );
		}
	}
}
