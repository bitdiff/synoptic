using System;
using System.IO;
using System.Text;

namespace Synoptic
{
    public sealed class Response
    {
        public Response()
        {
            ExitCode = 0;
            Stdout = new StringBuilder();
            Stderr = new StringBuilder();
        }

        public Response(string stdout)
            : this()
        {
            Out.WriteLine(stdout);
        }
        public Response(string stdout, params object[] objects)
            : this()
        {
            Out.WriteLine(stdout, objects);
        }

        public int ExitCode { get; set; }

        public StringBuilder Stdout { get; set; }
        public StringBuilder Stderr { get; set; }
        public TextWriter Out { get { return new StringWriter(Stdout); } }
        public TextWriter Error { get { return new StringWriter(Stderr); } }

        public string OutputText
        {
            get { return Stdout.ToString(); }
            set { Stdout = new StringBuilder(value); }
        }

        public string ErrorText
        {
            get { return Stderr.ToString(); }
            set { Stderr = new StringBuilder(value); }
        }

        public string Text
        {
            get { return OutputText; }
            set { OutputText = value; }
        }

        public int Execute()
        {
            return Execute(true);
        }

        public int Execute(bool exit)
        {
            if (Stdout.Length > 0) Console.Out.Write(Stdout.ToString());
            if (Stderr.Length > 0) Console.Error.Write(Stderr.ToString());
            if (exit) Environment.Exit(ExitCode);
            return ExitCode;
        }

        public Response Append(string input, params object[] objects) { return AppendToOutput(input, objects); }
        public Response Prepend(string input, params object[] objects) { return PrependToOutput(input, objects); }

        public Response AppendToOutput(string input, params object[] objects)
        {
            Stdout.Append(string.Format(input, objects));
            return this;
        }
        public Response PrependToOutput(string input, params object[] objects)
        {
            Stdout.Insert(0, string.Format(input, objects));
            return this;
        }

        public Response AppendToError(string input, params object[] objects)
        {
            Stderr.Append(string.Format(input, objects));
            return this;
        }
        public Response PrependToError(string input, params object[] objects)
        {
            Stderr.Insert(0, string.Format(input, objects));
            return this;
        }
    }
}