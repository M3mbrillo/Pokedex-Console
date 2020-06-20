using PokeApiNet;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex_Console
{
    internal class PokeConsole
    {
        private PokeApiClient pokeClient { get; } = new PokeApiClient();

        public PokeConsole()
        {

        }

        public async Task<Pokemon> FindAsync(string pokemonName)
        {
            try
            {
                return await this.pokeClient.GetResourceAsync<Pokemon>(pokemonName);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);                
            }
            return null;
        }


        public void WriteLineWithColor(string text, ConsoleColor foreColor, ConsoleColor backColor) {
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = foreColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public async Task ShowStatAsync(Pokemon pokemon)
        {
            var specie = await pokeClient.GetResourceAsync(pokemon.Species);

            var primaryColor = this.GetColor(await pokeClient.GetResourceAsync(specie.Color));

            this.WriteLineWithColor(new String(' ', Console.WindowWidth), primaryColor, primaryColor);          
            this.WriteLineWithColor(pokemon.Name, ConsoleColor.Black, ConsoleColor.White);

            await DrawerImage.FromUrl(pokemon.Sprites.FrontDefault);
        }


        public ConsoleColor GetColor(PokemonColor color)
        {
            ConsoleColor consoleColor = color.Name switch
            {
                "black" => ConsoleColor.Black,
                "blue" => ConsoleColor.Cyan,
                "brown" => ConsoleColor.DarkRed,
                "green" => ConsoleColor.Green,
                "gray" => ConsoleColor.Gray,
                "pink" => ConsoleColor.Magenta,
                "purple" => ConsoleColor.DarkMagenta,
                "red" => ConsoleColor.Red,
                "white" => ConsoleColor.White,
                "yellow" => ConsoleColor.Yellow,

                _ => ConsoleColor.Black
            };
            return consoleColor;
        }
    }
}