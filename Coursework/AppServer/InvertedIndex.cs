using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AppServer
{
	class InvertedIndex
	{
		private object mutex = new object();
		private string[] _filePaths;
		
		private const int ThreadCount = 4;
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
				var clearText = DeleteSpecialCharacters(text);
				var lexems = SplitString(clearText);
				for(var j=0;j<lexems.Length;j++){
					AddElement(lexems[j],_filePaths[i]);
				}
			}
		}
		
		private string DeleteSpecialCharacters(String text)
		{
				var sb = new StringBuilder();
				foreach (char c in text) {
					if ( c != '.' && c != ',') {
						sb.Append(c);
					}
				}
				return sb.ToString();
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