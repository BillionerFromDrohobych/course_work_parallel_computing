using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace AppServer
{
	class InvertedIndex
	{
		private object mutex = new object();
		private string[] _filePaths;
		
		private const int ThreadCount = 1;
		private const int FileCount = 2000;
		private readonly string DirectoryPath=@"C:\Users\1\Desktop\kursova\Coursework\Data";
		public void BuildIndex()
		{
			var threads = new Thread[ThreadCount];
			_filePaths = GetFilePaths();
			var sw = new Stopwatch();
				sw.Start();
			for (var i = 0; i < ThreadCount; i++)
			{
				var startIndexMultiplier = i;
				threads[i] = new Thread(new ThreadStart(() => ParallelBuildIndex(startIndexMultiplier)));
				threads[i].Start();
				
			}
			for (var i = 0; i < ThreadCount; i++)
			{
				threads[i].Join();
			}
			sw.Stop();
			Console.WriteLine("TimeToBuildIndex "+sw.Elapsed.TotalSeconds);
			Console.WriteLine("InvertedIndexReady");
		}
	
		private string[] GetFilePaths(){
			return Directory.GetFiles(DirectoryPath);
		}
		private void ParallelBuildIndex(int startIndexMultiplier){
			for (var i = FileCount*(startIndexMultiplier)/ThreadCount; i < FileCount*(startIndexMultiplier+1)/ThreadCount; i++)
			{
				var text = UploadFile(_filePaths[i]);
				DeleteAllSpecialCharacters();
				var lexems = SplitString(text);
				for(var j=0;j<lexems.Length;j++){
					AddElement(lexems[j],_filePaths[i]);
				}
			}
		}
		
		private void DeleteAllSpecialCharacters(){
			var reg = new Regex(@"[^0-9a-zA-Z]+");
			reg.Replace(@"[^0-9a-zA-Z]+", "");
		}
		private string[] SplitString(string text){
			return text.Split(' ');
		}
		private void AddElement(string lexem,string documentId){
			AppServer.InvertedIndex.AddOrUpdate(lexem,
			                          _ => new List<string>{documentId},
			                          (_, existing) =>
			                          {
			                          	lock (mutex)
			                          	{
			                          		if (!existing.Contains(documentId))
			                          		{
			                          			existing.Add(documentId);
			                          		}
			                          		return existing;
			                          	}
			                          });
			
		}
		private string UploadFile(string filePath){
			return File.ReadAllText(filePath);
		}
		
	}
}