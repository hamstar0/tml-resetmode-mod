using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using ResetMode.Data;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public override TagCompound Save() {
			var mymod = ResetModeMod.Instance;
			mymod.Session.Save();

			return base.Save();
		}


		public override void PreUpdate() {
			var mymod = ResetModeMod.Instance;
			ResetModeSessionData sessData = mymod.Session.Data;

			if( sessData.IsRunning ) {
				if( !sessData.AwaitingNextWorld ) {
					string worldId = WorldHelpers.GetUniqueId(true);

					if( sessData.CurrentSessionedWorldId == "" ) {
						LogHelpers.LogOnce( "!" + DebugHelpers.GetCurrentContext( 1 ) + " - Invalid world session state - "
							+"No world id (world id: "+worldId+")\n"
							+ mymod.Session.DataOnLoad.ToString() );
					} else if( sessData.CurrentSessionedWorldId != worldId ) {
						LogHelpers.LogOnce( "!" + DebugHelpers.GetCurrentContext( 1 ) + " - Invalid world session state - "
							+"Mismatched world id ("+sessData.CurrentSessionedWorldId+" vs "+worldId+")\n"
							+ mymod.Session.DataOnLoad.ToString() );
					}
				}
			}
		}
	}
}
