using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.UserHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStopCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 && !Main.dedServ ) {
					return CommandType.World;
				}
				return CommandType.Console | CommandType.World;
			}
		}
		public override string Command => "rm-stop";
		public override string Usage => "/"+this.Command;
		public override string Description => "Ends reset mode.";


		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			if( Main.netMode == 1 ) {
				LogHelpers.Log( "!ResetMode.Commands.ResetModeStopCommand - Not supposed to run on client." );
				return;
			}

			if( Main.netMode == 2 && caller.CommandType != CommandType.Console ) {
				bool hasPriv = UserHelpers.HasBasicServerPrivilege( caller.Player );

				if( !hasPriv ) {
					caller.Reply( "Access denied.", Color.Red );
					return;
				}
			}

			var mymod = (ResetModeMod)this.mod;
			
			if( ResetModeAPI.StopSession() ) {
				caller.Reply( "Reset mode ended.", Color.YellowGreen );
			} else {
				caller.Reply( "Reset mode is not in session.", Color.Red );
			}
		}
	}
}
