using System;
using System.IO;
using UnityEngine;
namespace Utils{
	public class Lexer:IDisposable{
		StreamReader input;
		MemoryStream stream;
		private string token;
		private TokenType tokenType;

		public enum TokenType{
			IdentifierToken,
			KeywordToken,
			IntToken,
			RealToken,
			StringToken,
			OtherToken,
			EndOfInput,
			IntRangeToken,
			RealRangeToken,
			ComponentToken,
			BoolToken,
			Vec3,
			Vec3Range
		}

		public Lexer (string fileInput){
			input = new StreamReader(GenerateStreamFromString(fileInput));
		}

		public Stream GenerateStreamFromString(string s){
			stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public void Dispose(){
			stream.Dispose();
		}
		private void SkipWhiteSpace(){
			int c = input.Peek();
			while(Char.IsWhiteSpace((char)c)){
				input.Read();
				if(input.EndOfStream)
					return;
				c = input.Peek();
			}
		}
		private int GetNextFromInput(){
			input.Read();
			return input.Peek();
		}

		private int GetNextNonWhiteSpaceFromInput(){
			input.Read();
			SkipWhiteSpace();
			return input.Peek();
		}

		public void NextToken(){
			try{
				token = "";
				SkipWhiteSpace();
				if(input.EndOfStream){
					token = "<eof>";
					tokenType = TokenType.EndOfInput;
					return;
				}
				int c = input.Peek();
				if(char.IsDigit((char)c)||c=='-'){
					if(c == '-'){
						token = token + (char) c;
						c = GetNextFromInput();
					}
					tokenType = TokenType.IntToken;
					while(char.IsDigit((char) c)){
						token = token + (char) c;
						c = GetNextFromInput();
					}
					if(c == '.'){
						tokenType = TokenType.RealToken;
						token = token + (char) c;
						c = GetNextFromInput();
						while(char.IsDigit((char) c)){
							token = token + (char) c;
							c = GetNextFromInput();
						}
					}
					if (c == ':'){
						token = token + (char) c;
						if(tokenType == TokenType.IntToken){
							tokenType = TokenType.IntRangeToken;
							c = GetNextFromInput();
							if(c == '-'){
								token = token + (char) c;
								c = GetNextFromInput();
							}
							while(char.IsDigit((char) c)){
								token = token + (char) c;
								c = GetNextFromInput();
							}
						}else {
							tokenType = TokenType.RealRangeToken;
							c = GetNextFromInput();
							if(c == '-'){
								token = token+(char) c;
								c = GetNextFromInput();
							}
							while(char.IsDigit((char) c)){
								token = token + (char) c;
								c = GetNextFromInput();
							}
							if(c == '.'){
								token = token + (char) c;
								c = GetNextFromInput ();
								while(char.IsDigit((char) c)){
									token = token + (char) c;
									c = GetNextFromInput();
								}
							}
						}
					}
					if(c == ',' &&
					   (tokenType == TokenType.IntToken || tokenType == TokenType.RealToken ||
					 	tokenType == TokenType.IntRangeToken || tokenType == TokenType.RealRangeToken)) {

					}
				}
			}catch{
			}
		}
	}
}

