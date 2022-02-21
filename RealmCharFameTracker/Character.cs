using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealmCharFameTracker
{
    class Character
		:
		NamedItem
	{
		public Character( string name,params string[] aliases )
			:
			base( name,aliases )
		{

		}
	}
}
