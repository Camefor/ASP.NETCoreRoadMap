namespace LambdaAndLINQ_MyDele1
{

    //声明委托:
    //委托类
    delegate void MyDele(); //独立在namespace外层，也说明了是一个类 类型
    //delegate int MyDele(int a, int b);

    class Student
    {
        public void SayHello()
        {
            Console.WriteLine("Hello, I'm a student!");
        }
    }

    class Program_MyDele1
    {
        //delegate void MyDele();//可以声明在class平级的地方（这里算是内部类吧……）


        static void Main_MyDele1(string[] args)
        {
            //学习委托,泛型委托,Lambda表达式,LINQ


            //委托类实例化：
            //dele1这个变量引用着一个MyDele类型（委托类）的实例，这个实例里“包裹着”M1这个方法
            MyDele dele1 = new MyDele(M1);
            Student stu = new();
            dele1 += M1;//多播委托
            dele1 += stu.SayHello;//多播委托
            dele1 += new Student().SayHello;//多播委托
            dele1(); //Invoke()


        }

        static void M1()
        {
            Console.WriteLine("M1 is called!");
        }
    }

}