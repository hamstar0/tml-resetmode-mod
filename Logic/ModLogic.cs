using HamstarHelpers.DebugHelpers;
using ResetMode.NetProtocol;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	partial class ModLogic {
		private int _NetMode = -1;
		public int NetMode { get {
			return this._NetMode == -1 ? Main.netMode : this._NetMode;
		} }



		////////////////

		public ModLogic() { }

		public void OnEnterGame() {
			this._NetMode = Main.netMode;
		}

		////////////////

		public bool IsSessionStarted( ResetModeMod mymod ) {
			return mymod.Session.IsRunning;
		}


		////////////////

		public void StartSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.StartSession" );
			}

			mymod.Session.IsRunning = true;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			if( mymod.Logic.NetMode == 2 ) {
				ServerPackets.SendSessionData( mymod, -1 );
			}

			this.StartSessionTimer( mymod );
		}
		
		public void ResumeSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.ResumeSession" );
			}

			mymod.Session.IsRunning = true;

			if( mymod.Logic.NetMode == 2 ) {
				ServerPackets.SendSessionData( mymod, -1 );
			}

			this.StartSessionTimer( mymod );
		}
		

		public void StopSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.StopSession IsSessionStarted: " + this.IsSessionStarted( mymod ) );
			}

			if( !this.IsSessionStarted( mymod ) ) { return; }

			mymod.Session.EndSession();

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void StartSessionTimer( ResetModeMod mymod ) {
			int time = mymod.Session.AwaitingNextWorld ? mymod.Config.SecondsUntilResetSubsequently : mymod.Config.SecondsUntilResetInitially;

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.ResumeSession time: " + time );
			}

			TimeLimitAPI.TimerStart( "reset", time, false );
		}
	}
}
