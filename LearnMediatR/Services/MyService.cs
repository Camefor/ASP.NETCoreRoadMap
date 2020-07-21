using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMediatR.Services {

    public interface IMyService {
        string SayHello();
    }
    public class MyService : IMyService {
        public string SayHello() {
            Console.WriteLine("Hello");
            return "Hello";
        }
    }
}
