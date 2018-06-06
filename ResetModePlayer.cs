using HamstarHelpers.TmlHelpers;
using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using ResetMode.Logic;
using ResetMode.NetProtocols;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;
		
		public bool HasModSettings { get; private set; }
		public bool HasSessionData { get; private set; }
		private bool HasEnteredWorld = false;



		////////////////

		public override bool CloneNewInstances { get { return false; } }

		public override void Initialize() {
			this.Logic = new PlayerLogic();
			this.HasModSettings = false;
			this.HasSessionData = false;
			this.HasEnteredWorld = false;
		}

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.HasModSettings = this.HasModSettings;
			clone.HasSessionData = this.HasSessionData;
			clone.HasEnteredWorld = this.HasEnteredWorld;
		}


		////////////////

		private void LoadGame() {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Session.Data.IsRunning ) {
				if( myworld.Logic.WorldStatus != ResetModeStatus.Normal ) {
					this.Logic.ValidatePlayer( mymod, this.player );
				}
			}
		}

		
		////////////////

		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 2 ) {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}
		
		public override void OnEnterWorld( Player player ) {
			var mymod = (ResetModeMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) {
				if( Main.netMode == 0 ) {
					if( !mymod.ConfigJson.LoadFile() ) {
						mymod.ConfigJson.SaveFile();
						ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
					}
				}

				if( Main.netMode == 1 ) {
					this.OnClientConnect();
				}
				if( Main.netMode == 0 ) {
					this.OnSingleConnect();
				}
			}
		}

		////////////////
		
		private void OnSingleConnect() {
			this.LoadGame();
			this.FinishModSettingsSync();
			this.FinishSessionSync();
		}

		private void OnClientConnect() {
			PacketProtocol.QuickRequestToServer<ModSettingsProtocol>();
			PacketProtocol.QuickRequestToServer<SessionProtocol>();
		}

		private void OnServerConnect() {
			this.HasModSettings = true;
			this.LoadGame();
		}


		////////////////

		public void FinishModSettingsSync() {
			this.HasModSettings = true;

			this.UpdateSync();
		}
		
		public void FinishSessionSync() {
			this.HasSessionData = true;

			this.UpdateSync();
		}

		////////////////

		public bool IsSynced() {
			return this.HasModSettings && this.HasSessionData;
		}

		public void UpdateSync() {
			if( this.IsSynced() && !this.HasEnteredWorld ) {
				this.HasEnteredWorld = true;
				
				this.Logic.OnEnterWorld( (ResetModeMod)this.mod, this.player );
			}
		}


		////////////////

		public override void PreUpdate() {
			var mymod = (ResetModeMod)this.mod;
			bool is_me = this.player.whoAmI == Main.myPlayer;

			if( is_me && this.IsSynced() ) {
				this.Logic.Update( mymod, this.player );
			}
		}
	}
}
