using HamstarHelpers.DebugHelpers;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageWorldForCurrentSession( ResetModeMod mymod ) {
			string world_id = Main.ActiveWorldFileData.UniqueId.ToString(); //WorldHelpers.GetUniqueId()

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EngageWorldForCurrentSession " + world_id );
			}

			if( !mymod.Session.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			mymod.Session.AddActiveWorld( world_id );
			
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Active;

			PlayerLogic.ValidateAll( mymod );
		}


		public void EndCurrentSession( ResetModeMod mymod ) {
			string world_id = Main.ActiveWorldFileData.UniqueId.ToString(); //WorldHelpers.GetUniqueId()

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EndCurrentSession " + world_id );
			}

			mymod.Session.ClearSessionData();
			
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Normal;

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void ExpireWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.CloseWorldForCurrentSession" );
			}

			mymod.Session.AwaitingNextWorld = true;
			
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}
	}
}
