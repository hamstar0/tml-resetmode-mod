using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal bool IsPlaying( ResetModeMod mymod, Player player ) {
			string uid = PlayerIdentityHelpers.GetProperUniqueId( player );

			return this.Data.PlayersValidated.Contains( uid );
		}


		internal void AddPlayer( ResetModeMod mymod, Player player ) {
			string uid = PlayerIdentityHelpers.GetProperUniqueId( player );

			this.Data.PlayersValidated.Add( uid );
			if( Main.netMode != 1 ) {
				this.Save( mymod );
			}
		}
	}
}
