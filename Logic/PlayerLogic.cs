using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static int GetHash( Player player ) {
			return PlayerIdentityHelpers.GetVanillaSnapshotHash( player, true, false );
		}



		////////////////
		
		public void OnEnterWorld( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			switch( myworld.Logic.WorldStatus ) {
			case ResetModeStatus.Normal:
				this.Welcome( mymod, player );
				break;
			case ResetModeStatus.Active:
				this.Instruct( player );
				break;
			}
		}


		////////////////

		/*public void Update( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( this.IsPrompting ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
				player.immuneTime = 2;
			}
		}*/


		////////////////

		public void Welcome( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 0 ) {
				Main.NewText( "Type /resetmodestart to start Reset Mode. Type /help for a list of other available commands.", Color.LightGray );
			} else {
				Main.NewText( "Type resetmodestart in the server console to start Reset Mode.", Color.LightGray );
			}
		}

		public void Unwelcome( Player player ) {
			Main.NewText( "Warning: " + player.name + " is already playing reset mode elsewhere. You risk losing your progress.", Color.Red );
		}

		public void Instruct( Player player ) {
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
