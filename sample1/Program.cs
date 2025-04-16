using System;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

public class Personne
{
    public string Nom { get; set; }
    public int Age { get; set; }

    public string Hello(bool isLowercase)
    {
        string message = $"hello {Nom}, you are {Age}";
        return isLowercase ? message : message.ToUpper();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Partie JSON
        Personne p = new Personne { Nom = "Alice", Age = 30 };
        string json = JsonConvert.SerializeObject(p, Formatting.Indented);
        Console.WriteLine("🎯 Données JSON :\n" + json);

        // Partie ImageSharp
        string inputPath = "image-originale.jpg";
        string outputPath = "image-redimensionnee.jpg";

        try
        {
            using (Image image = Image.Load(inputPath))
            {
                Console.WriteLine($"📷 Image chargée : {image.Width}x{image.Height}");

                // Redimensionnement
                image.Mutate(x => x.Resize(200, 200));
                image.Save(outputPath, new JpegEncoder());

                Console.WriteLine($"✅ Image redimensionnée sauvegardée sous : {outputPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors du traitement de l'image : {ex.Message}");
        }
    }
}
