using PokeApiNet;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Pokedex_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Pokedex Console !");

            Pokemon pokemon = null;
            var pokeConsole = new PokeConsole();

            while (pokemon == null)
            {

                Console.Write("Pokemon name> ");
                var pokemonName = Console.ReadLine();


                pokemon = await pokeConsole.FindAsync(pokemonName);

                if (pokemon != null)
                    await pokeConsole.ShowStatAsync(pokemon);
            }

        }
    }
}
