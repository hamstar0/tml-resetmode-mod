using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using TimeLimit;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EngageWorldForCurrentSession " + WorldHelpers.GetUniqueId() );
			}

			if( !mymod.Session.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			mymod.Session.AllPlayedWorlds.Add( WorldHelpers.GetUniqueId() );
			mymod.Session.AwaitingNextWorld = false;
			mymod.Session.IsRunning = true;
			
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Active;

			PlayerLogic.ValidateAll( mymod );
		}


		public void EndCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EndCurrentSession " + WorldHelpers.GetUniqueId() );
			}

			mymod.Session.AllPlayedWorlds.Clear();
			mymod.Session.AwaitingNextWorld = false;
			mymod.Session.IsRunning = false;
			
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
