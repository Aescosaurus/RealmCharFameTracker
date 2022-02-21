using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RealmCharFameTracker
{
	class Dungeon
		:
		NamedItem
	{
		class SaveItem
		{
			public SaveItem( float dur,int fame,int charUsed,int charMax,
				bool solo,bool rush,bool completed,bool second,int minionXPBoost,int bossXPBoost )
			{
				duration = dur;
				fameEarned = fame;
				this.charUsed = charUsed;
				this.charMax = charMax;

				this.solo = solo;
				this.rush = rush;
				this.completed = completed;
				this.second = second;
				this.minionXPBoost = minionXPBoost;
				this.bossXPBoost = bossXPBoost;
			}

			public float duration;
			public int fameEarned;
			public int charUsed;
			public int charMax;
			public bool solo;
			public bool rush;
			public bool completed;
			public bool second;
			public int minionXPBoost;
			public int bossXPBoost;
		}

		public Dungeon( string name,bool hasSecond,params string[] aliases )
			:
			base( name,aliases )
		{
			this.hasSecond = hasSecond;

			LoadSaveItems();
		}

		public void ReloadSaveItems()
		{
			saveItems.Clear();

			LoadSaveItems();
		}

		void LoadSaveItems()
		{
			if( !File.Exists( GetSaveName() ) ) return;

			var reader = new StreamReader( GetSaveName() );
			var lines = new List<string>();
			while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
			reader.Close();
			
			foreach( var line in lines )
			{
				var vals = line.Split( ',' );
				
				saveItems.Add( new SaveItem( float.Parse( vals[0] ),
					int.Parse( vals[1] ),
					int.Parse( vals[2] ),
					int.Parse( vals[3] ),
					int.Parse( vals[4] ) == 1,
					int.Parse( vals[5] ) == 1,
					int.Parse( vals[6] ) == 1,
					int.Parse( vals[7] ) == 1,
					int.Parse( vals[8] ),
					int.Parse( vals[9] ) ) );
			}
		}

		public void AddSaveItem( float duration,int fameEarned,int charUsed,int charMax,
				bool solo,bool rush,bool completed,bool second,int minionXPBoost,int bossXPBoost )
		{
			saveItems.Add( new SaveItem( duration,fameEarned,charUsed,charMax,
				solo,rush,completed,second,minionXPBoost,bossXPBoost ) );
			SaveSaveItems();
		}

		void SaveSaveItems()
		{
			if( !Directory.Exists( saveFolder ) ) Directory.CreateDirectory( saveFolder );

			string result = "";
			foreach( var saveItem in saveItems )
			{
				result += saveItem.duration.ToString() + ',' +
					saveItem.fameEarned.ToString() + ',' +
					saveItem.charUsed.ToString() + ',' +
					saveItem.charMax.ToString() + ',' +
					( saveItem.solo ? '1' : '0' ) + ',' +
					( saveItem.rush ? '1' : '0' ) + ',' +
					( saveItem.completed ? '1' : '0' ) + ',' +
					( saveItem.second ? '1' : '0' ) + ',' +
					saveItem.minionXPBoost.ToString() + ',' +
					saveItem.bossXPBoost.ToString() + '\n';
			}

			var writer = new StreamWriter( GetSaveName() );
			writer.Write( result );
			writer.Close();
		}

		string GetSaveName()
		{
			return( saveFolder + GetName() + ".txt" );
		}

		public bool HasStats()
		{
			return( saveItems.Count > 0 );
		}

		public float CalcAvgTime()
		{
			float total = 0.0f;
			foreach( var item in saveItems )
			{
				total += item.duration;
			}
			return( total / saveItems.Count );
		}

		public float CalcAvgFame()
		{
			float total = 0.0f;
			foreach( var item in saveItems )
			{
				total += item.fameEarned;
			}
			return ( total / saveItems.Count );
		}

		public float CalcAvgFpm()
		{
			float total = 0.0f;
			foreach( var item in saveItems )
			{
				total += item.fameEarned / item.duration;
			}
			return ( total / saveItems.Count );
		}

		public bool HasSecond()
		{
			return( hasSecond );
		}

		List<SaveItem> saveItems = new List<SaveItem>();

		bool hasSecond = false;

		static readonly string saveFolder = "Dungeons/";
	}
}
