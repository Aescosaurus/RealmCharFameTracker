using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace RealmCharFameTracker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
		:
		Window
	{
		public MainWindow()
		{
			InitializeComponent();

			UpdateFilteredDungeons();

			UpdateDungeonCombo( DungeonList,null );
			UpdateCharacterCombo( CharacterList,null );

			UpdateStartEnabled();
			UpdateFinishEnabled();

			UpdateDungeonCombo( DungeonStatsCombo,null );

			// UpdateBestFPMList();
			UpdateSortList();

			UpdateDungeonCombo( SpecificDungeonList,null );
			UpdateCharacterCombo( CharStatsCombo,null );
		}

		private void DungeonSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateDungeonCombo( DungeonList,DungeonSearch );
		}

		void UpdateDungeonCombo( ComboBox target,TextBox search )
		{
			target.Items.Clear();

			foreach( var dungeon in filteredDungeons )
			{
				if( search == null || dungeon.NameMatch( search.Text ) )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = dungeon.GetName();

					target.Items.Add( comboBoxItem );
				}
			}
		}

		void UpdateCharacterCombo( ComboBox target,TextBox search )
		{
			target.Items.Clear();

			foreach( var character in chars )
			{
				if( search == null || character.NameMatch( search.Text ) )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = character.GetName();

					target.Items.Add( comboBoxItem );
				}
			}
		}

		private void Fame_PreviewTextInput( object sender,TextCompositionEventArgs args )
		{
			args.Handled = intRegex.IsMatch( args.Text );
		}

		private void DungeonSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				// select 1st item in list
				UpdateDungeonCombo( DungeonList,DungeonSearch );
				if( DungeonList.Items.Count > 0 ) DungeonList.SelectedIndex = 0;

				StartingFame.Focus();
			}
		}

		private void CharacterSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateCharacterCombo( CharacterList,CharacterSearch );
		}

		private void CharacterSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				UpdateCharacterCombo( CharacterList,CharacterSearch );
				if( CharacterList.Items.Count > 0 ) CharacterList.SelectedIndex = 0;

				MaxAmount.Focus();
			}
		}

		private void MaxAmount_PreviewTextInput( object sender,TextCompositionEventArgs args )
		{
			args.Handled = maxRegex.IsMatch( args.Text );
		}

		void UpdateStartEnabled()
		{
			StartButton.IsEnabled = DungeonList.SelectedIndex > -1 &&
				StartingFame.Text.Length > 0 &&
				CharacterList.SelectedIndex > -1 &&
				MaxAmount.Text.Length == 1 &&
				!started;
		}

		void UpdateFinishEnabled()
		{
			EndButton.IsEnabled = EndFame.Text.Length > 0 &&
				started;
		}

		private void CancelButton_Click( object sender,RoutedEventArgs e )
		{
			started = false;
			RunningText.Text = "";

			DungeonSearch.Text = "";
			StartingFame.Text = "";
			DungeonList.SelectedIndex = -1;
			EndFame.Text = "";

			DungeonSearch.Focus();

			UpdateStartEnabled();
		}

		private void CheckStartSelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			UpdateStartEnabled();
		}

		private void CheckStartTextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateStartEnabled();
		}

		private void MaxAmount_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return ) DungeonSearch.Focus();
		}

		private void StartingFame_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return && StartButton.IsEnabled )
			{
				Start();

				EndFame.Focus();
			}
		}

		private void StartButton_Click( object sender,RoutedEventArgs e )
		{
			Start();
		}

		void Start()
		{
			startTime = DateTime.Now;
			started = true;
			StartButton.IsEnabled = false;

			RunningText.Text = "running";
		}

		private void EndFame_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateFinishEnabled();
		}

		private void EndFame_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return && EndButton.IsEnabled )
			{
				Finish();

				DungeonSearch.Focus();
			}
		}

		private void EndButton_Click( object sender,RoutedEventArgs e )
		{
			Finish();
		}

		void Finish()
		{
			var totalTime = ( float )( DateTime.Now - startTime ).TotalMinutes;
			var totalFame = int.Parse( EndFame.Text ) - int.Parse( StartingFame.Text );
			int charUsed = CharName2Index( ( CharacterList.SelectedItem as ComboBoxItem ).Content.ToString() );
			var maxAmount = int.Parse( MaxAmount.Text );

			int dungeonUsed = -1;
			var selectedDungeonName = ( DungeonList.SelectedItem as ComboBoxItem ).Content.ToString();
			for( int i = 0; i < filteredDungeons.Count; ++i )
			{
				if( filteredDungeons[i].GetName() == selectedDungeonName )
				{
					dungeonUsed = i;
					break;
				}
			}

			var selectedDungeon = filteredDungeons[dungeonUsed];
			selectedDungeon.AddSaveItem( totalTime,totalFame,charUsed,maxAmount );

			DungeonSearch.Text = "";
			DungeonList.SelectedIndex = -1;
			StartingFame.Text = "";
			EndFame.Text = "";
			started = false;
			RunningText.Text = "";
			UpdateStartEnabled();

			UpdateDungeonStatsList();
			// UpdateBestFPMList();
			UpdateSortList();
		}

		private void StatsSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateDungeonCombo( DungeonStatsCombo,StatsSearch );
		}

		private void StatsSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				// select 1st item in list
				UpdateDungeonCombo( DungeonStatsCombo,StatsSearch );
				if( DungeonStatsCombo.Items.Count > 0 ) DungeonStatsCombo.SelectedIndex = 0;
			}
		}

		private void DungeonStatsCombo_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			if( DungeonStatsCombo.SelectedItem == null ) return;

			UpdateDungeonStatsList();

			SpecificDungeonList.SelectedIndex = DungeonName2Index( ( DungeonStatsCombo.SelectedItem as ComboBoxItem ).Content.ToString() );
		}

		void UpdateDungeonStatsList()
		{
			DungeonStatsList.Items.Clear();

			if( DungeonStatsCombo.SelectedItem == null ) return;

			var selectedDungeon = filteredDungeons[DungeonName2Index( ( DungeonStatsCombo.SelectedItem as ComboBoxItem ).Content.ToString() )];
			selectedDungeon.ReloadSaveItems();

			if( selectedDungeon.HasStats() )
			{
				AddStat( "Avg Time: " + selectedDungeon.CalcAvgTime( curChar ).ToString( "0.00" ) + 'm' );
				AddStat( "Avg Fame: " + selectedDungeon.CalcAvgFame( curChar ).ToString( "0.00" ) );
				AddStat( "Avg f/m: " + selectedDungeon.CalcAvgFPM( curChar ).ToString( "0.00" ) );
				AddStat( "Times Completed: " + selectedDungeon.CountDungeonsCompletedByChar( curChar ).ToString() );
				AddStat( "Highest Fame: " + selectedDungeon.GetHighestFame( curChar ).ToString() );
				AddStat( "Quickest Run: " + selectedDungeon.GetQuickestRun( curChar ).ToString( "0.00" ) + 'm' );
				AddStat( "Best FPM: " + selectedDungeon.GetBestFPM( curChar ).ToString( "0.00" ) );
			}
			else AddStat( "No data" );
		}

		void AddStat( string name )
		{
			var newItem = new ListBoxItem();
			newItem.Content = name;
			DungeonStatsList.Items.Add( newItem );
		}

		private void SortOptionCombo_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			if( SortList != null ) UpdateSortList();
		}

		void UpdateSortList()
		{
			var sortedDungeons = new List<Dungeon>();
			foreach( var dungeon in filteredDungeons )
			{
				if( dungeon.HasStats() ) sortedDungeons.Add( dungeon );
			}

			sortedDungeons.Sort( delegate( Dungeon a,Dungeon b )
			{
				return( sortLambs[SortOptionCombo.SelectedIndex]( a,b,curChar ) );
			} );

			SortList.Items.Clear();

			for( int i = 0; i < sortedDungeons.Count; ++i )
			{
				var listBoxItem = new ListBoxItem();
				listBoxItem.Content = sortedListLambs[SortOptionCombo.SelectedIndex]( sortedDungeons[i],curChar );
				listBoxItem.Tag = DungeonName2Index( sortedDungeons[i].GetName() ).ToString();

				SortList.Items.Add( listBoxItem );
			}
		}

		private void SortList_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			if( SortList.SelectedIndex > -1 )
			{
				// StatsSearch.Text = ( SortOptionCombo.SelectedValue as ComboBoxItem ).Content.ToString();
				StatsSearch.Text = "";
				UpdateDungeonCombo( DungeonStatsCombo,StatsSearch );

				int selectedDungeonIndex = int.Parse( ( SortList.SelectedValue as ListBoxItem ).Tag.ToString() );

				DungeonStatsCombo.SelectedIndex = selectedDungeonIndex;

				SpecificDungeonList.SelectedIndex = selectedDungeonIndex;
			}
		}

		int DungeonName2Index( string dungeonName )
		{
			for( int i = 0; i < filteredDungeons.Count; ++i )
			{
				if( filteredDungeons[i].GetName() == dungeonName ) return( i );
			}
			return( -1 );
		}

		int CharName2Index( string charName )
		{
			for( int i = 0; i < chars.Length; ++i )
			{
				if( chars[i].GetName() == charName ) return( i );
			}
			return( -1 );
		}

		private void SpecificDungeon_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			UpdateSpecificSortList();
		}

		void UpdateSpecificSortList()
		{
			if( SpecificSortList == null ) return;

			SpecificSortList.Items.Clear();

			if( SpecificDungeonList.SelectedIndex < 0 ) return;

			var sortedEntries = new List<Dungeon.SaveItem>();
			sortedEntries.AddRange( filteredDungeons[SpecificDungeonList.SelectedIndex].GetCharSpecificStats( curChar ) );

			sortedEntries.Sort( delegate( Dungeon.SaveItem a,Dungeon.SaveItem b )
			{
				return( specificSortLambs[SpecificSortCombo.SelectedIndex]( a,b ) );
			} );

			for( int i = 0; i < sortedEntries.Count; ++i )
			{
				var listBoxItem = new ListBoxItem();
				listBoxItem.Content = specificSortedListLambs[SpecificSortCombo.SelectedIndex]( sortedEntries[i] );
				// listBoxItem.Tag = DungeonName2Index( sortedDungeons[i].GetName() ).ToString();

				SpecificSortList.Items.Add( listBoxItem );
			}
		}

		private void CharStatsSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateCharacterCombo( CharStatsCombo,CharStatsSearch );
		}

		private void CharStatsSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				UpdateCharacterCombo( CharStatsCombo,CharStatsSearch );
				if( CharStatsCombo.Items.Count > 0 ) CharStatsCombo.SelectedIndex = 0;
			}
		}

		private void CharStatsCombo_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			UpdateCharStatsList();

			UpdateCurChar();

			if( filterDungeons ) UpdateDungeonBoxesByFilter();
		}

		void UpdateCurChar()
		{
			curChar = -1;
			if( filterDungeons && CharStatsCombo.SelectedIndex > -1 )
			{
				curChar = CharName2Index( ( CharStatsCombo.SelectedValue as ComboBoxItem ).Content.ToString() );
			}
		}

		void UpdateCharStatsList()
		{
			CharStatsList.Items.Clear();

			if( CharStatsCombo.SelectedItem == null ) return;

			int selectedChar = CharName2Index( ( CharStatsCombo.SelectedValue as ComboBoxItem ).Content.ToString() );
			AddCharStatsListItem( "Dungeons Completed: " + CountTotalDungeonsCompletedByChar( selectedChar ).ToString() );
			AddCharStatsListItem( "Avg f/m: " + CalcAvgFPMByChar( selectedChar ).ToString( "0.00" ) );
			AddCharStatsListItem( "Avg fame: " + CalcAvgFameByChar( selectedChar ).ToString( "0.00" ) );
			AddCharStatsListItem( "Avg time: " + CalcAvgTimeByChar( selectedChar ).ToString( "0.00" ) );
		}

		int CountTotalDungeonsCompletedByChar( int character )
		{
			int total = 0;
			foreach( var dungeon in filteredDungeons )
			{
				total += dungeon.CountDungeonsCompletedByChar( character );
			}
			return( total );
		}

		float CalcAvgFPMByChar( int character )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var dungeon in filteredDungeons )
			{
				if( dungeon.HasStatsForChar( character ) )
				{
					total += dungeon.CalcAvgFPM( character );
					++byChar;
				}
			}
			return( total / byChar );
		}

		float CalcAvgFameByChar( int character )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var dungeon in filteredDungeons )
			{
				if( dungeon.HasStatsForChar( character ) )
				{
					total += dungeon.CalcAvgFame( character );
					++byChar;
				}
			}
			return( total / byChar );
		}

		float CalcAvgTimeByChar( int character )
		{
			float total = 0.0f;
			int byChar = 0;
			foreach( var dungeon in filteredDungeons )
			{
				if( dungeon.HasStatsForChar( character ) )
				{
					total += dungeon.CalcAvgTime( character );
					++byChar;
				}
			}
			return( total / byChar );
		}

		void AddCharStatsListItem( string text )
		{
			var listItem = new ListBoxItem();
			listItem.Content = text;

			CharStatsList.Items.Add( listItem );
		}

		private void FilterDungeonCheck_Click( object sender,RoutedEventArgs e )
		{
			filterDungeons = FilterDungeonCheck.IsChecked ?? false;

			UpdateCurChar();

			UpdateDungeonBoxesByFilter();
		}

		void UpdateDungeonBoxesByFilter()
		{
			UpdateFilteredDungeons();

			DungeonStatsCombo.SelectedIndex = -1;
			SpecificDungeonList.SelectedIndex = -1;
			UpdateDungeonCombo( DungeonStatsCombo,StatsSearch );
			UpdateDungeonCombo( SpecificDungeonList,null );
			// UpdateCharacterCombo( CharStatsCombo,CharStatsSearch );
			// if( CharStatsCombo.Items.Count > 0 ) CharStatsCombo.SelectedIndex = 0;

			UpdateSortList();
			UpdateDungeonStatsList();
			UpdateSpecificSortList();
		}

		void UpdateFilteredDungeons()
		{
			filteredDungeons.Clear();

			int selectedChar = -1;
			if( CharStatsCombo.SelectedItem != null )
			{
				selectedChar = CharName2Index( ( CharStatsCombo.SelectedValue as ComboBoxItem ).Content.ToString() );
			}

			foreach( var dungeon in dungeons )
			{
				if( !filterDungeons || selectedChar < 0 || dungeon.HasStatsForChar( selectedChar ) )
				{
					filteredDungeons.Add( dungeon );
				}
			}
		}

		Regex intRegex = new Regex( "[^0-9]+" );
		Regex maxRegex = new Regex( "[^0-8]+" );

		DateTime startTime = DateTime.Now;
		bool started = false;

		bool filterDungeons = false;
		List<Dungeon> filteredDungeons = new List<Dungeon>();
		int curChar = -1;

		Func<Dungeon,Dungeon,int,int>[] sortLambs =
		{
			( Dungeon a,Dungeon b,int curChar ) => b.CalcAvgFPM( curChar ).CompareTo( a.CalcAvgFPM( curChar ) ),
			( Dungeon a,Dungeon b,int curChar ) => b.CalcAvgFame( curChar ).CompareTo( a.CalcAvgFame( curChar ) ),
			( Dungeon a,Dungeon b,int curChar ) => a.CalcAvgTime( curChar ).CompareTo( b.CalcAvgTime( curChar ) ),
			( Dungeon a,Dungeon b,int curChar ) => b.CountDungeonsCompletedByChar( curChar ).CompareTo( a.CountDungeonsCompletedByChar( curChar ) ),
		};

		Func<Dungeon.SaveItem,Dungeon.SaveItem,int>[] specificSortLambs =
		{
			( Dungeon.SaveItem a,Dungeon.SaveItem b ) => b.CalcFPM().CompareTo( a.CalcFPM() ),
			( Dungeon.SaveItem a,Dungeon.SaveItem b ) => b.fameEarned.CompareTo( a.fameEarned ),
			( Dungeon.SaveItem a,Dungeon.SaveItem b ) => a.duration.CompareTo( b.duration ),
		};

		Func<Dungeon,int,string>[] sortedListLambs =
		{
			( Dungeon d,int curChar ) => ( d.GetName() + ": " + d.CalcAvgFPM( curChar ).ToString( "0.00" ) + " (" +
				d.CalcAvgFame( curChar ).ToString( "0.00" ) + " / " + d.CalcAvgTime( curChar ).ToString( "0.00" ) + ")" ),
			( Dungeon d,int curChar ) => ( d.GetName() + ": " + d.CalcAvgFame( curChar ).ToString( "0.00" ) + " (" +
				d.CalcAvgFPM( curChar ).ToString( "0.00" ) + "fpm)" ),
			( Dungeon d,int curChar ) => ( d.GetName() + ": " + d.CalcAvgTime( curChar ).ToString( "0.00" ) + " (" +
				d.CalcAvgFPM( curChar ).ToString( "0.00" ) + "fpm)" ),
			( Dungeon d,int curChar ) => ( d.GetName() + ": " + d.CountDungeonsCompletedByChar( curChar ).ToString() + " (" +
				d.CalcAvgFPM( curChar ).ToString( "0.00" ) + "fpm)" )
		};

		Func<Dungeon.SaveItem,string>[] specificSortedListLambs =
		{
			( Dungeon.SaveItem e ) => ( e.CalcFPM().ToString( "0.00" ) + "( " +
				e.fameEarned.ToString() + " / " + e.duration.ToString( "0.00" ) + ")" ),
			( Dungeon.SaveItem e ) => ( e.fameEarned.ToString() + " (" + e.CalcFPM().ToString( "0.00" ) + "fpm)" ),
			( Dungeon.SaveItem e ) => ( e.duration.ToString( "0.00" ) + " (" + e.CalcFPM().ToString( "0.00" ) + "fpm)" )
		};

		static readonly Dungeon[] dungeons =
		{
			new Dungeon( "Pirate Cave","pcave" ),
			new Dungeon( "Forest Maze" ),
			new Dungeon( "Spider Den" ),
			new Dungeon( "Forbidden Jungle" ),
			new Dungeon( "The Hive" ),
			new Dungeon( "Candyland Hunting Grounds","cland" ),
			new Dungeon( "Ancient Ruins" ),
			new Dungeon( "Magic Woods","mwoods" ),
			new Dungeon( "Snake Pit" ),
			new Dungeon( "Sprite World" ),
			new Dungeon( "Cave of a Thousand Treasures","tcave" ),
			new Dungeon( "Undead Lair","udl" ),
			new Dungeon( "Manor of the Immortals" ),
			new Dungeon( "Puppet Master's Theatre" ),
			new Dungeon( "Toxic Sewers" ),
			new Dungeon( "Cursed Library" ),
			new Dungeon( "Mad Lab" ),
			new Dungeon( "Abyss of Demons","abby" ),
			new Dungeon( "Haunted Cemetary" ),
			new Dungeon( "The Machine" ),
			new Dungeon( "The Inner Workings" ),
			new Dungeon( "The Crawling Depths","cdepths" ),
			new Dungeon( "Parasite Chambers" ),
			new Dungeon( "Woodland Labyrinth","wlab" ),
			new Dungeon( "Deadwater Docks","ddocks" ),
			new Dungeon( "Beachzone" ),
			new Dungeon( "Davy Jones' Locker","davys" ),
			new Dungeon( "Ice Cave" ),
			new Dungeon( "Mountain Temple" ),
			new Dungeon( "Lair of Draconis","lod" ),
			new Dungeon( "Ocean Trench","ot" ),
			new Dungeon( "Tomb of the Ancients" ),
			new Dungeon( "The Third Dimension" ),
			new Dungeon( "Fungal Cavern" ),
			new Dungeon( "Crystal Cavern" ),
			new Dungeon( "The Nest" ),
			new Dungeon( "Lost Halls","lh" ),
			new Dungeon( "Cultist Hideout" ),
			new Dungeon( "The Void" ),
			new Dungeon( "The Shatters","shats" ),
			new Dungeon( "Malogia" ),
			new Dungeon( "Forax" ),
			new Dungeon( "Untaris" ),
			new Dungeon( "Katalund" ),
			new Dungeon( "Oryx's Chamber" ),
			new Dungeon( "Wine Cellar","o2" ),
			new Dungeon( "Oryx's Sanctuary" ),
			new Dungeon( "Puppet Master's Encore" ),
			new Dungeon( "Lair of Shaitan" ),
			new Dungeon( "Cnidarian Reef" ),
			new Dungeon( "Secluded Thicket" ),
			new Dungeon( "High Tech Terror" ),
			new Dungeon( "Heroic Undead Lair" ),
			new Dungeon( "Heroic Abyss of Demons" ),
			new Dungeon( "Rainbow Road" ),
			new Dungeon( "Santa's Workshop" ),
			new Dungeon( "Ice Tomb" ),
			new Dungeon( "Battle for the Nexus" ),
			new Dungeon( "Belladonna's Garden" ),
			new Dungeon( "Mad God Mayhem" )
		};

		static readonly Character[] chars =
		{
			new Character( "Rogue" ),
			new Character( "Archer" ),
			new Character( "Wizard","wizzy" ),
			new Character( "Priest" ),
			new Character( "Warrior" ),
			new Character( "Knight" ),
			new Character( "Paladin","pally" ),
			new Character( "Assassin" ),
			new Character( "Necromancer" ),
			new Character( "Huntress" ),
			new Character( "Mystic" ),
			new Character( "Trickster" ),
			new Character( "Sorcerer" ),
			new Character( "Ninja" ),
			new Character( "Samurai" ),
			new Character( "Bard" ),
			new Character( "Summoner" ),
			new Character( "Kensei" ),
		};
	}
}
