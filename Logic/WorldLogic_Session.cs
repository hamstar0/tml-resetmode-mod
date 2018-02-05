using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
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

			if( Main.netMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.WorldStatus = ResetModeStatus.Active;
		}

		public void ExpireWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.CloseWorldForCurrentSession" );
			}

			mymod.Session.AwaitingNextWorld = true;

			if( Main.netMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}

		////////////////

		public void ClearAllSessionWorlds( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.ClearAllSessionWorlds" );
			}
			
			mymod.Session.AllPlayedWorlds.Clear();
			mymod.Session.AwaitingNextWorld = false;

			if( Main.netMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.WorldStatus = ResetModeStatus.Normal;
		}
	}
}
