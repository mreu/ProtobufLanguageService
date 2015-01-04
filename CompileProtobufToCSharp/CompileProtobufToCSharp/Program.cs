namespace CompileProtobufToCSharp
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Any())
            {
                var c = new Compile();
                c.Run(args[0]);
            }
        }
    }
}
