using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RealmCharFameTracker
{
	class NamedItem
	{
		public NamedItem( string name,params string[] aliases )
		{
			this.name = name;
			foreach( var alias in aliases )
			{
				this.aliases.Add( alias );
			}
		}

		public string GetName()
		{
			return ( name );
		}

		public bool NameMatch( string str )
		{
			if( name.ToUpper().Contains( str.ToUpper() ) ) return ( true );
			else
			{
				foreach( var alias in aliases )
				{
					if( alias.ToUpper().Contains( str.ToUpper() ) ) return ( true );
				}
			}

			return ( false );
		}

		protected string name;
		protected List<string> aliases = new List<string>();
	}
}
