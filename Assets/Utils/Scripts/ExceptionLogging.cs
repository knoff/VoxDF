using UnityEngine;
using System.Collections;
using System.IO;
namespace Utils{
	public class ExceptionLogging : MonoBehaviour {
		public string errorFile = @"error.log";
		public string logFile = @"log.log";
		public bool log = false;
		private StringWriter logWriter;
	
		void OnEnable() {
			Application.logMessageReceived+=ExceptionWriter;
		}
	
		void OnDisable() {
			Application.logMessageReceived-=ExceptionWriter;
		}
	
		//This is an instance of a delegate, we'll cover these in more detail in the Advanced scripting chapter
		//Changing the types or order of the types in this method will break the code.
		//You can name it whatever you like.
		void ExceptionWriter(string logString, string stackTrace, LogType type) {
			switch(type) {
			case LogType.Exception:
			case LogType.Error:
				using(StreamWriter writer = new StreamWriter(new FileStream(errorFile, FileMode.Append))) {
					writer.WriteLine(type);
					writer.WriteLine(logString);
					writer.WriteLine(stackTrace);
				}
				break;
			case LogType.Log:
				if(log){
					using(StreamWriter writer = new StreamWriter(new FileStream(logFile, FileMode.Append))) {
						writer.WriteLine(type);
						writer.WriteLine(logString);
						writer.WriteLine(stackTrace);
					}
				}
				break;
			default:
				break;
			}
		}
		/*public void Log(string logString, LogType type){
			switch(type){
				LogType.Log
				
			}
		}*/
	}
}