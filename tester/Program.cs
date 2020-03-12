using SQLGenerator;
using System;
using System.ComponentModel.DataAnnotations;

namespace tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(SqlClassGenerator.GetTable<MyObject>());
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(SqlClassGenerator.GetView<MyObject>());
            Console.WriteLine();
            Console.WriteLine();
            var p = SqlClassGenerator.GetParams<MyObject>();
            Console.WriteLine(string.Join("\r\n",p));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(SqlClassGenerator.GetInsertStatement<MyObject>());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(SqlClassGenerator.GetUpdateStatement<MyObject>());

            Console.ReadLine();
        }
    }

    class MyObject
    {
        public Guid ID { get; set; }
        public string Note { get; set; }
        [MaxLength(20)]
        public string str20 { get; set; }
        public int Number { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
