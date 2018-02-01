using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStopCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "resetmodestop"; } }
		public override string Usage { get { return "/resetmodestop"; } }
		public override string Description { get { return "Ends reset mode."; } }


		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( !mymod.Logic.IsSessionStarted( mymod ) ) {
				caller.Reply( "Reset mode is not in session.", Color.Red );
				return;
			}

			mymod.Logic.StopSession( mymod );
			myworld.Logic.ClearAllSessionWorlds( mymod );

			caller.Reply( "Reset mode ended!.", Color.YellowGreen );
		}
	}
}
