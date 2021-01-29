using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace NachoSolver
{
    internal class Program
    {
        //Cuerpo del programa
        private static void Main(string[] args)
        {
            //Cargar el string, del array indicado en el argumento (0)
            var sourceFileContent = File.ReadAllText(args[0]);

            //Convertir el string (JSON) en un array bidimensional de caracteres
            var sourceArray = JsonConvert.DeserializeObject<char[][]>(sourceFileContent);

            //Crear el diccionario donde se van a alojar los rankings de las letras
            var resultsDictionary = new Dictionary<char, uint>();

            //Recorrer el array
            for (uint y = 0; y < sourceArray.Length; y++)
            {
                for (uint x = 0; x < sourceArray[y].Length; x++)
                {
                    var character = sourceArray[y][x];

                    var startingVector = new[]
                    {
                        y,
                        x
                    };

                    //Obtener la cantidad de ocurrencias
                    var rightCount = CountSequentialSameCharacter(sourceArray, startingVector, z => z + 1, z => z);
                    UpdateDictionaryWithValues(resultsDictionary, character, rightCount);

                    var bottomCount = CountSequentialSameCharacter(sourceArray, startingVector, z => z, z => z + 1);
                    UpdateDictionaryWithValues(resultsDictionary, character, bottomCount);

                    var bottomRightCount = CountSequentialSameCharacter(sourceArray, startingVector, z => z + 1, z => z + 1);                    
                    UpdateDictionaryWithValues(resultsDictionary, character, bottomRightCount);

                    var bottomLeftCount = CountSequentialSameCharacter(sourceArray, startingVector, z => z - 1, z => z + 1);
                    UpdateDictionaryWithValues(resultsDictionary, character, bottomLeftCount);
                }
            }

            //Ordenar el diccionario por sus valores, de manera descendente y luego, tomar el primer registro (el que mas ocurrencias tiene)
            var topResult = resultsDictionary.OrderByDescending(keySelector => keySelector.Value)
                .ToDictionary(keySelector => keySelector.Key, elementSelector => elementSelector.Value)
                .First();

            //Mostrar el caracter con mas repeticiones consecutivas, luego mostrar la cantidad de elementos adyacentes
            Console.WriteLine($"Top result: {topResult.Key} with {topResult.Value + 1} adjacent elements.");
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        //Actualizar el diccionario, con el caracter actual y la cantidad de ocurrencias encontradas
        private static void UpdateDictionaryWithValues(IDictionary<char, uint> destinationDictionary, char character, uint count)
        {
            if (destinationDictionary.ContainsKey(character))
            {
                //SI. Si el valor actual (en el diccionario) es MENOR, reemplazar, caso contrario, no hacer nada
                destinationDictionary[character] = destinationDictionary[character] < count ? count : destinationDictionary[character];
            }
            else
            {
                //NO. Agregarla con las ocurrencias
                destinationDictionary.Add(character, count);
            }
        }

        //Contar la cantidad de caracteres iguales
        public static uint CountSequentialSameCharacter(char[][] matrix, uint[] rootVector, Func<uint, uint> xFunction, Func<uint, uint> yFunction)
        {
            var rootValue = matrix[rootVector[0]][rootVector[1]];
            uint matchCounter = 0;
            var nextVector = new uint[2];
            nextVector[0] = yFunction(rootVector[0]);
            nextVector[1] = xFunction(rootVector[1]);

            while (IsBetweenBoundaries(matrix, nextVector))
            {
                if (rootValue == matrix[nextVector[0]][nextVector[1]])
                {
                    matchCounter++;
                    nextVector[0] = yFunction(nextVector[0]);
                    nextVector[1] = xFunction(nextVector[1]);
                }
                else
                {
                    break;
                }
            }


            return matchCounter;
        }

        //Funcion que verifica si el vector indicado esta dentro de la matriz
        private static bool IsBetweenBoundaries(char[][] matrix, uint[] vector)
        {
            return vector[1] < matrix.Length && vector[0] < matrix[vector[1]].Length;
        }
    }
}