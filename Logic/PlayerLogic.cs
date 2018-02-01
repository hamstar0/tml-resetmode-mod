using HamstarHelpers.DebugHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.WorldHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public bool IsPrompting = false;
		public bool IsValidating = false;

		private bool HasShownWelcome = false;
		private bool HasShownInstructions = false;

		private int SessionSyncTimer = 0;



		////////////////

		public void Update( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			
			if( !this.IsValidating ) {
				if( mymod.Logic.IsSessionStarted(mymod) ) {
					if( myworld.Logic.WorldStatus == ResetModeStatus.Active ) {
						this.ValidatePlayer( mymod, player );
					}
				}
			}

			bool is_sessioned = !string.IsNullOrEmpty( this.ActiveForWorldUid );

			if( mymod.Logic.NetMode != 2 ) {
				if( !this.HasShownWelcome && !this.IsPrompting ) {
					if( is_sessioned ) {
						if( this.ActiveForWorldUid != WorldHelpers.GetUniqueId() ) {
							if( !myworld.Logic.IsWorldUidInSession( mymod, this.ActiveForWorldUid ) ) {
								this.Unwelcome( player );
							}
						}
					} else {
						this.Welcome( mymod, player );
					}

					if( !this.HasShownInstructions && !this.IsPrompting ) {
						if( myworld.Logic.WorldStatus == ResetModeStatus.Active ) {
							this.Instruct( player );
						}
					}
				}
			}

			if( mymod.Logic.NetMode != 1 ) {
				if( --this.SessionSyncTimer <= 0 ) {
					this.SessionSyncTimer = 180;
					
					if( is_sessioned ) {
						if( this.ActiveForWorldUid == WorldHelpers.GetUniqueId() ) {
							mymod.Logic.RegisterPlayerDataWithSession( mymod, player );
						}
					}
				}
			}

			if( this.IsPrompting ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
				player.immuneTime = 2;
			}
		}


		////////////////
		
		public void Welcome( ResetModeMod mymod, Player player ) {
			this.HasShownWelcome = true;
			
			if( mymod.Logic.NetMode == 0 ) {
				Main.NewText( "Type /resetmodestart to start Reset Mode. Type /help for a list of other available commands.", Color.LightGray );
			} else {
				Main.NewText( "Type resetmodestart in the server console to start Reset Mode.", Color.LightGray );
			}
		}

		public void Unwelcome( Player player ) {
			this.HasShownWelcome = true;

			Main.NewText( "Warning: "+player.name+" is already playing reset mode elsewhere. You risk losing your progress.", Color.Red );
		}

		public void Instruct( Player player ) {
			this.HasShownInstructions = true;

			Main.NewText( "Welcome to Reset Mode. After the timer, you and this world reset. Only your Progress Points (PP) are kept for the next.", Color.Cyan );
		}


		////////////////
		
		public void Boot( ResetModeMod mymod, Player player, string reason ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.Boot player: " + player.whoAmI );
			}

			ErrorLogger.Log( player.name + " was booted because " + reason );
			TmlHelpers.ExitToMenu( true );
		}
	}
}
