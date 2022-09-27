using UnityEngine;
using System;
public class CSVReader : MonoBehaviour
{
    //Resources.Load later
    [SerializeField] TextAsset _csvFile;
    void Awake()
    {
        
    }

    //Read and Update Command Dictionary
    void UpdateTextAsset()
    {
        //check metadata for changes?
        //write any new data to spreadsheet
        //save changes
    }

    //Maybe Excel is just a record of all avaliable commands (Read-Only)
    //Have callbacks be readonly - how to turn variable name into a string?
    void ReadCSV()
    {
        //Split commas - Aspect of a Command
        //Split carriage returns - Next Command
        string[] csvData = _csvFile.text.Split(new string[] { ",", "\n"}, StringSplitOptions.None);

        int tableSize = (csvData.Length / 2) - 1;

        for (int i = 0; i < tableSize; i++)
        {
            //if(CommandManager.Commands.ContainsKey(csvData[2 * (1 + i)]))
                //continue;

            //data for one command
        }
    }
}
