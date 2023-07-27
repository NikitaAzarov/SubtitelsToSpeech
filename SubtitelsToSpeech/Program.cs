using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SubtitelsToSpeech
{
    internal class Program
    {
        //Set Speech key and region from environment variables named "SPEECH_KEY" and "SPEECH_REGION"
        static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
        static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

        // Output Speech Synthesis Result
        static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                default:
                    break;
            }
        }

        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff48Khz16BitMonoPcm);
            using var speechSynthesizer = new SpeechSynthesizer(speechConfig, null);

            var configuration = new ConfigurationBuilder().SetBasePath("E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech").AddIniFile("Settings.ini").Build();
            string lang_code = configuration.GetSection("Language-2")["lang-code"];
            string lang_voice = configuration.GetSection("Language-2")["Voice"];
            string style_voice = configuration.GetSection("Language-2")["Style"];
            string style_degree = configuration.GetSection("Language-2")["style_degree"];
            string effect_voice = configuration.GetSection("Language-2")["effect"];
            string srtFilePatch = "E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech\\" + configuration.GetSection("Settings")["srt_file_path"];

            string SilenceLeadingExact = "<mstts:silence  type=\"Leading-exact\" value=\"0ms\"/>";
            string SilenceTailingExact = "<mstts:silence  type=\"Tailing-exact\" value=\"0ms\"/>";
            string VoiceStyle = string.Format("<mstts:express-as style=\"{0}\" styledegree=\"\"/>", style_voice, style_degree);

            Captions subtitles = new Captions(srtFilePatch);
            int countSubtitles = subtitles.GetAmount();

            for (int i = 0; i < countSubtitles; i++)
            {
                Console.WriteLine("Make Synthesizer " + (i+1) + " in " + countSubtitles);
                string ssml = string.Format("<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xml:lang=\"{0}\">", lang_code);
                ssml += string.Format("<voice name=\"{0}\" effect=\"{1}\">", lang_voice, effect_voice);
                ssml += SilenceLeadingExact + SilenceTailingExact;
                ssml += string.Format("<mstts:express-as style=\"{0}\" styledegree=\"{1}\"/>", style_voice, style_degree);
                ssml += string.Format("<mstts:audioduration value=\"{0}\"/>", subtitles.GetSubtitle(i).DurationMilliSeconds());
                ssml += subtitles.GetSubtitle(i).GetText() + "</voice></speak>";
                var result = await speechSynthesizer.SpeakSsmlAsync(ssml);
                using var stream = AudioDataStream.FromResult(result);
                await stream.SaveToWaveFileAsync(string.Format("E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech\\temp\\{0}.wav", i+1));
            }
            Console.WriteLine("creating Output file");
            string outputFilePath = "E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech\\Temp\\output.wav";
            string[] inputFiles = new string[Directory.GetFiles("E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech\\Temp").Length];
            for (int i = 0; i < inputFiles.Length; i++)
                inputFiles[i] = string.Format("E:\\Projects Visual Studio\\SubtitelsToSpeech\\SubtitelsToSpeech\\Temp\\{0}.wav", i + 1);

            // Создание списка для хранения источников звука
            List<ISampleProvider> inputs = new List<ISampleProvider>();

            foreach (string inputFile in inputFiles)
            {
                // Открытие каждого WAV-файла в качестве источника звука
                AudioFileReader audioFile = new AudioFileReader(inputFile);
                inputs.Add(audioFile);
            }

            // Объединение источников звука
            ConcatenatingSampleProvider concatenatingProvider = new ConcatenatingSampleProvider(inputs);

            // Создание выходного WAV-файла
            WaveFileWriter.CreateWaveFile16(outputFilePath, concatenatingProvider);     
            
            Console.WriteLine("Finish");




        }

    }
}