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
		public class SaveItem
		{
			public SaveItem( float dur,int fame,int charUsed,int charMax )
			{
				duration = dur;
				fameEarned = fame;
				this.charUsed = charUsed;
				this.charMax = charMax;
			}

			public float CalcFPM()
			{
				return( fameEarned / duration );
			}

			public float duration;
			public int fameEarned;
			public int charUsed;
			public int charMax;
		}

		public Dungeon( string name,params string[] aliases )
			:
			base( name,aliases )
		{
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
				// string dur = "";
				// string fame = "";
				// bool finishDur = false;
				// foreach( char c in line )
				// {
				// 	if( c == ',' ) finishDur = true;
				// 	else if( finishDur ) fame += c;
				// 	else dur += c;
				// }
				var vals = line.Split( ',' );
				
				saveItems.Add( new SaveItem( float.Parse( vals[0] ),
					int.Parse( vals[1] ),
					int.Parse( vals[2] ),
					int.Parse( vals[3] ) ) );
			}
		}

		public void AddSaveItem( float duration,int fameEarned,int charUsed,int charMax )
		{
			saveItems.Add( new SaveItem( duration,fameEarned,charUsed,charMax ) );
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
					saveItem.charMax.ToString() + '\n';
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

		public float CalcAvgTime( int charUsed = -1 )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					total += item.duration;
					++byChar;
				}
			}
			return( total / byChar );
		}

		public float CalcAvgFame( int charUsed = -1 )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					total += item.fameEarned;
					++byChar;
				}
			}
			return ( total / byChar );
		}

		public float CalcAvgFPM( int charUsed = -1 )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					total += item.CalcFPM();
					++byChar;
				}
			}
			return( total / byChar );
		}

		// public int GetTimesCompleted()
		// {
		// 	return( saveItems.Count );
		// }

		public int GetHighestFame( int charUsed = -1 )
		{
			int highest = -1;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					if( item.fameEarned > highest ) highest = item.fameEarned;
				}
			}
			return( highest );
		}

		public float GetQuickestRun( int charUsed = -1)
		{
			float quickest = float.MaxValue;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					if( item.duration < quickest ) quickest = item.duration;
				}
			}
			return( quickest );
		}

		public float GetBestFPM( int charUsed = -1 )
		{
			float best = -1.0f;
			foreach( var item in saveItems )
			{
				if( charUsed < 0 || item.charUsed == charUsed )
				{
					var curFPM = item.CalcFPM();
					if( curFPM > best ) best = curFPM;
				}
			}
			return( best );
		}

		// public List<SaveItem> GetAllItemsList()
		// {
		// 	return( saveItems );
		// }

		public int CountDungeonsCompletedByChar( int character = -1 )
		{
			int total = 0;
			foreach( var item in saveItems )
			{
				if( character < 0 || item.charUsed == character ) ++total;
			}
			return( total );
		}

		public bool HasStatsForChar( int character )
		{
			foreach( var item in saveItems )
			{
				if( item.charUsed == character ) return( true );
			}
			return( false );
		}

		public List<SaveItem> GetCharSpecificStats( int character )
		{
			var specificStats = new List<SaveItem>();

			foreach( var item in saveItems )
			{
				if( character < 0 || item.charUsed == character ) specificStats.Add( item );
			}

			return( specificStats );
		}

		private Dungeon()
			:
			base( "" )
		{}

		public Dungeon GetCharSpecificDungeon( int character )
		{
			var newDungeon = new Dungeon();
			newDungeon.name = name;
			newDungeon.aliases = aliases;

			newDungeon.saveItems = GetCharSpecificStats( character );

			return( newDungeon );
		}

		List<SaveItem> saveItems = new List<SaveItem>();

		static readonly string saveFolder = "Dungeons/";
	}
}
