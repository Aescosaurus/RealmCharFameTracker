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

			UpdateDungeonSearchItems();
			UpdateCharSearchItems();

			UpdateStartEnabled();
			UpdateFinishEnabled();

			UpdateDungeonStatsSearchItems();

			UpdateBestFPMList();
		}

		private void DungeonSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateDungeonSearchItems();
		}

		void UpdateDungeonSearchItems()
		{
			DungeonList.Items.Clear();

			foreach( var dungeon in dungeons )
			{
				if( dungeon.NameMatch( DungeonSearch.Text ) )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = dungeon.GetName();

					DungeonList.Items.Add( comboBoxItem );
				}
			}
		}

		void UpdateCharSearchItems()
		{
			CharacterList.Items.Clear();

			foreach( var character in chars )
			{
				if( character.NameMatch( CharacterSearch.Text ) )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = character.GetName();

					CharacterList.Items.Add( comboBoxItem );
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
				UpdateDungeonSearchItems();
				if( DungeonList.Items.Count > 0 ) DungeonList.SelectedIndex = 0;

				StartingFame.Focus();
			}
		}

		private void CharacterSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateCharSearchItems();
		}

		private void CharacterSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				UpdateCharSearchItems();
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
			if( e.Key == Key.Return )
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
			if( e.Key == Key.Return )
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
			int charUsed = -1;
			var selectedCharName = ( CharacterList.SelectedItem as ComboBoxItem ).Content.ToString();
			for( int i = 0; i < chars.Length; ++i )
			{
				if( chars[i].GetName() == selectedCharName )
				{
					charUsed = i;
					break;
				}
			}
			var maxAmount = int.Parse( MaxAmount.Text );

			int dungeonUsed = -1;
			var selectedDungeonName = ( DungeonList.SelectedItem as ComboBoxItem ).Content.ToString();
			for( int i = 0; i < dungeons.Length; ++i )
			{
				if( dungeons[i].GetName() == selectedDungeonName )
				{
					dungeonUsed = i;
					break;
				}
			}

			var selectedDungeon = dungeons[dungeonUsed];
			selectedDungeon.AddSaveItem( totalTime,totalFame,charUsed,maxAmount );

			DungeonSearch.Text = "";
			DungeonList.SelectedIndex = -1;
			StartingFame.Text = "";
			EndFame.Text = "";
			started = false;
			RunningText.Text = "";
			UpdateStartEnabled();

			UpdateDungeonStatsCombo();
			UpdateBestFPMList();
		}

		private void StatsSearch_TextChanged( object sender,TextChangedEventArgs e )
		{
			UpdateDungeonStatsSearchItems();
		}

		private void StatsSearch_KeyDown( object sender,KeyEventArgs e )
		{
			if( e.Key == Key.Return )
			{
				// select 1st item in list
				UpdateDungeonStatsSearchItems();
				if( DungeonStatsCombo.Items.Count > 0 ) DungeonStatsCombo.SelectedIndex = 0;
			}
		}

		void UpdateDungeonStatsSearchItems()
		{
			DungeonStatsCombo.Items.Clear();

			foreach( var dungeon in dungeons )
			{
				if( dungeon.NameMatch( StatsSearch.Text ) )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = dungeon.GetName();

					DungeonStatsCombo.Items.Add( comboBoxItem );
				}
			}
		}

		private void DungeonStatsCombo_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			UpdateDungeonStatsCombo();
		}

		void UpdateDungeonStatsCombo()
		{
			DungeonStatsList.Items.Clear();

			if( DungeonStatsCombo.SelectedItem == null ) return;

			int dungeonUsed = -1;
			var selectedDungeonName = ( DungeonStatsCombo.SelectedItem as ComboBoxItem ).Content.ToString();
			for( int i = 0; i < dungeons.Length; ++i )
			{
				if( dungeons[i].GetName() == selectedDungeonName )
				{
					dungeonUsed = i;
					break;
				}
			}

			var selectedDungeon = dungeons[dungeonUsed];
			selectedDungeon.ReloadSaveItems();

			if( selectedDungeon.HasStats() )
			{
				AddStat( "Avg Time: " + selectedDungeon.CalcAvgTime().ToString( "0.00" ) + 'm' );
				AddStat( "Avg Fame: " + selectedDungeon.CalcAvgFame().ToString( "0.00" ) );
				AddStat( "Avg f/m: " + selectedDungeon.CalcAvgFpm().ToString( "0.00" ) );
			}
			else AddStat( "No data" );
		}

		void AddStat( string name )
		{
			var newItem = new ListBoxItem();
			newItem.Content = name;
			DungeonStatsList.Items.Add( newItem );
		}

		void UpdateBestFPMList()
		{
			var sortedDungeons = new List<Dungeon>();
			foreach( var dungeon in dungeons ) sortedDungeons.Add( dungeon );

			sortedDungeons.Sort( delegate( Dungeon a,Dungeon b )
			{
				return( b.CalcAvgFpm().CompareTo( a.CalcAvgFpm() ) );
			} );


			BestFPMList.Items.Clear();

			foreach( var dungeon in sortedDungeons )
			{
				if( dungeon.HasStats() )
				{
					var comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = dungeon.GetName() + ": " + dungeon.CalcAvgFpm().ToString( "0.00" ) +
						"(" + dungeon.CalcAvgFame().ToString( "0.00" ) + " / " +
						dungeon.CalcAvgTime().ToString( "0.00" ) + ")";

					BestFPMList.Items.Add( comboBoxItem );
				}
			}
		}

		Regex intRegex = new Regex( "[^0-9]+" );
		Regex maxRegex = new Regex( "[^0-8]+" );

		DateTime startTime = DateTime.Now;
		bool started = false;

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
