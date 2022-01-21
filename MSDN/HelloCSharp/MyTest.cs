using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp
{
    public class Animal
    {

    }

    /// <summary>
    /// 哺乳动物
    /// </summary>
    public class Mammal : Animal
    {

    }
    public class Dog : Mammal
    {

    }



    public class MyTest
    {


        public static void MyFunc()
        {
            //基类可以包含派生类，
            Animal animal1 = new Animal();
            Animal animal2 = new Mammal(); //哺乳动物是动物
            Animal animal3 = new Dog(); //狗是动物
            Mammal animal4 = new Dog(); //狗是哺乳动物
            //但是派生类不能包含基类
            //Mammal animal5 = new Animal(); // 编译错误。 动物不一定就是哺乳动物
            //Dog animal6 = new Mammal(); // 编译错误。 哺乳动物不一定就是狗

            /**
             * 协变使您能够在需要基类型的地方传递派生类型。
             *基类和其他派生类被认为是向基类型添加额外功能的同一类。
             * 因此， 协变允许您在期望基类的情况下使用派生类（规则：如果期望小，则可以接受大）。
             *  协变可以应用于委托、泛型、数组、接口等。
             * **/



            //委托中
            //委托中的协变允许委托方法的返回类型具有灵活性。
            //协变允许您将方法分配给具有较少派生返回类型（Mammal， Dog ）的委托。
            //EatFood eatFood = Methond1;
            //Animal mammal = eatFood(new Mammal());

            //eatFood = Methond2;
            //Animal mammal2 = eatFood(new Mammal());


            //Console.WriteLine();
            //Console.WriteLine();

            ////逆变器应用于参数。协变允许将具有基类参数的方法分配给期望派生类参数的委托。

            //EatFood eatFood2 = Methond1;
            //eatFood2 += Methond2;
            //eatFood2 += Methond3;
            //Animal mammal_new = eatFood2(new Mammal());


            Con_EatFood eat = Con_Methond1;
            var d = eat(new Animal());

        }

        public static Mammal Con_Methond1(Animal animal)
        {

            Console.WriteLine("处理数据代码");
            return new Mammal();
        }


        public static Mammal Methond1(Mammal mammal)
        {
            Console.WriteLine("Methond1");
            Console.WriteLine("哺乳动物吃饭……");
            return new Mammal();
        }
        public static Animal Methond2(Mammal mammal)
        {
            Console.WriteLine("Methond2");
            Console.WriteLine("狗吃饭……");
            return new Animal();
        }

        public static Animal Methond3(Animal animal)
        {
            Console.WriteLine("Methond3");
            Console.WriteLine("动物吃饭……");
            return new Animal();
        }



        public delegate Animal EatFood(Mammal an);


        public delegate Animal Con_EatFood(Animal an);

    }
}
